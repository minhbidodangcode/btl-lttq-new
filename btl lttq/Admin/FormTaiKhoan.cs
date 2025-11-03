using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using ClosedXML.Excel;


namespace btl_lttq.Admin
{
    public partial class FormTaiKhoan : Form
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["MessengerDb"].ConnectionString;

        private bool isAdding = false;

        private string currentPasswordHash = null;
        private string currentPasswordSalt = null;

        private readonly string adminSecretsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "admin_passwords.dat");

        private Dictionary<string, string> adminSecretsCache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public FormTaiKhoan()
        {
            InitializeComponent();
            LoadAdminSecretsFromFile();
        }

        private void FormTaiKhoan_Load(object sender, EventArgs e)
        {
            LoadUsers();

            dgvUsers.CellClick += dgvUsers_CellClick;
            dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsers.MultiSelect = false;

            // Nút xuất Excel
            var btnExportXlsx = new Button
            {
                Text = "Xuất Excel (.xlsx)",
                Width = 120,
                Height = 24,
                Left = 18,
                Top = 292
            };
            btnExportXlsx.Click += btnExportXlsx_Click;
            this.Controls.Add(btnExportXlsx);

            // (ĐÃ BỎ nút CSV và toàn bộ logic CSV)

            LockControls();
        }

        // ============ Xuất Excel bằng ClosedXML ============
        private void btnExportXlsx_Click(object sender, EventArgs e)
        {
            if (dgvUsers.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất.");
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Title = "Lưu Excel";
                sfd.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
                sfd.FileName = "Users.xlsx";
                if (sfd.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    ExportGridToXlsx(dgvUsers, sfd.FileName);
                    if (MessageBox.Show("Đã xuất. Mở file ngay?", "Xuất Excel",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Xuất Excel thất bại: " + ex.Message);
                }
            }
        }

        private void ExportGridToXlsx(DataGridView grid, string path)
        {
            // cần reference ClosedXML (ClosedXML.Excel)
            using (var wb = new ClosedXML.Excel.XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Users");

                // Header
                int col = 1;
                foreach (DataGridViewColumn c in grid.Columns)
                {
                    ws.Cell(1, col).Value = c.HeaderText;
                    ws.Cell(1, col).Style.Font.Bold = true;
                    ws.Cell(1, col).Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;
                    ws.Cell(1, col).Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                    col++;
                }

                // Data
                int row = 2;
                foreach (DataGridViewRow r in grid.Rows)
                {
                    if (r.IsNewRow) continue;
                    for (int c = 0; c < grid.Columns.Count; c++)
                    {
                        var val = r.Cells[c].Value;
                        ws.Cell(row, c + 1).Value = val?.ToString();
                    }
                    row++;
                }

                // Auto-fit
                ws.Columns().AdjustToContents();

                wb.SaveAs(path);
            }
        }

        #region Admin secrets file handling (DPAPI)
        private void LoadAdminSecretsFromFile()
        {
            try
            {
                if (File.Exists(adminSecretsFile))
                {
                    string json = File.ReadAllText(adminSecretsFile, Encoding.UTF8);
                    adminSecretsCache = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                }
            }
            catch
            {
                adminSecretsCache = new Dictionary<string, string>();
            }
        }

        private void SaveAdminSecretsToFile()
        {
            try
            {
                string json = JsonSerializer.Serialize(adminSecretsCache);
                File.WriteAllText(adminSecretsFile, json, Encoding.UTF8);
                try { File.SetAttributes(adminSecretsFile, FileAttributes.Hidden); } catch { }
            }
            catch (Exception ex)
            {
               
            }
        }

        private string ProtectString(string plain)
        {
            if (string.IsNullOrEmpty(plain)) return string.Empty;
            byte[] plainBytes = Encoding.UTF8.GetBytes(plain);
            byte[] cipher = ProtectedData.Protect(plainBytes, null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(cipher);
        }

        private string UnprotectString(string protectedBase64)
        {
            if (string.IsNullOrEmpty(protectedBase64)) return string.Empty;
            try
            {
                byte[] cipher = Convert.FromBase64String(protectedBase64);
                byte[] plain = ProtectedData.Unprotect(cipher, null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(plain);
            }
            catch
            {
                return string.Empty;
            }
        }

        private void SavePlainPasswordForAdmin(string userId, string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(userId)) return;
            if (plainPassword == null) plainPassword = string.Empty;
            string protectedStr = ProtectString(plainPassword);
            adminSecretsCache[userId] = protectedStr;
            SaveAdminSecretsToFile();
        }

        private string GetPlainPasswordForAdmin(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return string.Empty;
            if (adminSecretsCache.TryGetValue(userId, out string protectedBase64))
            {
                return UnprotectString(protectedBase64);
            }
            return string.Empty;
        }
        #endregion

        private void LoadUsers()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            Id,
                            UserName,
                            Email,
                            CAST(CONVERT(varchar(max), PasswordHash) AS nvarchar(max)) AS PasswordHash,
                            CAST(CONVERT(varchar(max), PasswordSalt) AS nvarchar(max)) AS PasswordSalt,
                            DisplayName,
                            AvatarUrl,
                            StatusText,
                            IsActive,
                            CONVERT(varchar(30), CreatedAt, 121) AS CreatedAt,
                            CONVERT(varchar(30), UpdatedAt, 121) AS UpdatedAt
                        FROM dbo.Users
                        ORDER BY CreatedAt DESC";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvUsers.DataSource = dt;
                    dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgvUsers.ReadOnly = true;
                    dgvUsers.AllowUserToAddRows = false;

                    dgvUsers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                    dgvUsers.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
                    dgvUsers.EnableHeadersVisualStyles = false;
                    dgvUsers.AlternatingRowsDefaultCellStyle.BackColor = Color.AliceBlue;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi khi tải dữ liệu: " + ex.Message);
            }
        }

        private void dgvUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvUsers.Rows[e.RowIndex];

            txtId.Text = row.Cells["Id"].Value?.ToString();
            txtUserName.Text = row.Cells["UserName"].Value?.ToString();
            txtEmail.Text = row.Cells["Email"].Value?.ToString();

            // Lưu hash thật vào biến ẩn
            currentPasswordHash = row.Cells["PasswordHash"].Value?.ToString();
            currentPasswordSalt = row.Cells["PasswordSalt"].Value?.ToString();

            txtDisplayName.Text = row.Cells["DisplayName"].Value?.ToString();
            txtAvatarUrl.Text = row.Cells["AvatarUrl"].Value?.ToString();
            txtStatusText.Text = row.Cells["StatusText"].Value?.ToString();
            txtCreatedAt.Text = row.Cells["CreatedAt"].Value?.ToString();
            txtUpdatedAt.Text = row.Cells["UpdatedAt"].Value?.ToString();

            bool isActive = false;
            if (row.Cells["IsActive"].Value != DBNull.Value && row.Cells["IsActive"].Value != null)
            {
                isActive = Convert.ToBoolean(row.Cells["IsActive"].Value);
            }
            chkIsActive.Checked = isActive;

            // Thử lấy mật khẩu gốc (nếu admin đã lưu cục bộ trước đó)
            string userId = txtId.Text;
            string plainPassword = GetPlainPasswordForAdmin(userId);
            txtPasswordHash.Text = plainPassword; // nếu rỗng → textbox trống

            // Salt
            string plainSalt = GetPlainPasswordForAdmin(userId + "_salt");
            txtPasswordSalt.Text = plainSalt;

            LockControls();
        }

        private void LockControls()
        {
            txtId.ReadOnly = true;
            txtUserName.ReadOnly = true;
            txtEmail.ReadOnly = true;
            txtPasswordHash.ReadOnly = true;
            txtPasswordSalt.ReadOnly = true;
            txtDisplayName.ReadOnly = true;
            txtAvatarUrl.ReadOnly = true;
            txtStatusText.ReadOnly = true;
            chkIsActive.Enabled = false;
        }

        private void UnlockControls()
        {
            txtUserName.ReadOnly = false;
            txtEmail.ReadOnly = false;
            txtPasswordHash.ReadOnly = false; // admin có thể nhập pass mới ở đây
            txtPasswordSalt.ReadOnly = false;
            txtDisplayName.ReadOnly = false;
            txtAvatarUrl.ReadOnly = false;
            txtStatusText.ReadOnly = false;
            chkIsActive.Enabled = true;
        }

        private void ClearFields()
        {
            txtId.Clear();
            txtUserName.Clear();
            txtEmail.Clear();
            txtPasswordHash.Clear();
            txtPasswordSalt.Clear();
            txtDisplayName.Clear();
            txtAvatarUrl.Clear();
            txtStatusText.Clear();
            txtCreatedAt.Clear();
            txtUpdatedAt.Clear();
            chkIsActive.Checked = false;

            currentPasswordHash = null;
            currentPasswordSalt = null;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            isAdding = true;
            ClearFields();

            txtId.Text = Guid.NewGuid().ToString().ToUpper();

            UnlockControls();
            txtId.ReadOnly = true;
            txtUserName.Focus();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("⚠️ Hãy chọn người dùng cần sửa!");
                return;
            }

            isAdding = false;
            UnlockControls();
            txtId.ReadOnly = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra Id
            if (!Guid.TryParse(txtId.Text.Trim(), out Guid userId))
            {
                MessageBox.Show("❌ Id không hợp lệ (không phải GUID).");
                return;
            }

            // 2. Xử lý mật khẩu
            string passwordToSaveHash;
            string saltToSaveHash;

            if (isAdding)
            {
                if (string.IsNullOrWhiteSpace(txtPasswordHash.Text))
                {
                    MessageBox.Show("⚠️ Vui lòng nhập mật khẩu cho người dùng mới!");
                    return;
                }
                passwordToSaveHash = HashPassword(txtPasswordHash.Text);
                saltToSaveHash = HashPassword(txtPasswordSalt.Text ?? string.Empty);

                // Lưu bản plaintext đã mã hoá cho admin
                SavePlainPasswordForAdmin(userId.ToString(), txtPasswordHash.Text);
                SavePlainPasswordForAdmin(userId.ToString() + "_salt", txtPasswordSalt.Text ?? string.Empty);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(txtPasswordHash.Text))
                {
                    passwordToSaveHash = HashPassword(txtPasswordHash.Text);
                    SavePlainPasswordForAdmin(userId.ToString(), txtPasswordHash.Text);
                }
                else
                {
                    passwordToSaveHash = currentPasswordHash ?? string.Empty;
                }

                if (!string.IsNullOrWhiteSpace(txtPasswordSalt.Text))
                {
                    saltToSaveHash = HashPassword(txtPasswordSalt.Text);
                    SavePlainPasswordForAdmin(userId.ToString() + "_salt", txtPasswordSalt.Text);
                }
                else
                {
                    saltToSaveHash = currentPasswordSalt ?? string.Empty;
                }
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd;

                if (isAdding)
                {
                    cmd = new SqlCommand(@"
                        INSERT INTO dbo.Users
                        (Id, UserName, Email, PasswordHash, PasswordSalt, DisplayName, AvatarUrl, StatusText, IsActive, CreatedAt)
                        VALUES
                        (@Id, @UserName, @Email,
                         CONVERT(varbinary(max), @PasswordHash),
                         CONVERT(varbinary(max), @PasswordSalt),
                         @DisplayName, @AvatarUrl, @StatusText, @IsActive, SYSDATETIME())
                    ", conn);
                }
                else
                {
                    cmd = new SqlCommand(@"
                        UPDATE dbo.Users SET
                            UserName = @UserName,
                            Email = @Email,
                            PasswordHash = CONVERT(varbinary(max), @PasswordHash),
                            PasswordSalt = CONVERT(varbinary(max), @PasswordSalt),
                            DisplayName = @DisplayName,
                            AvatarUrl = @AvatarUrl,
                            StatusText = @StatusText,
                            IsActive = @IsActive,
                            UpdatedAt = SYSDATETIME()
                        WHERE Id = @Id
                    ", conn);
                }

                // Tham số
                cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = userId;
                cmd.Parameters.Add("@UserName", SqlDbType.NVarChar, 255).Value =
                    string.IsNullOrEmpty(txtUserName.Text) ? (object)DBNull.Value : txtUserName.Text;
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 255).Value =
                    string.IsNullOrEmpty(txtEmail.Text) ? (object)DBNull.Value : txtEmail.Text;

                cmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar).Value = passwordToSaveHash ?? string.Empty;
                cmd.Parameters.Add("@PasswordSalt", SqlDbType.NVarChar).Value = saltToSaveHash ?? string.Empty;

                cmd.Parameters.Add("@DisplayName", SqlDbType.NVarChar, 255).Value =
                    string.IsNullOrEmpty(txtDisplayName.Text) ? (object)DBNull.Value : txtDisplayName.Text;
                cmd.Parameters.Add("@AvatarUrl", SqlDbType.NVarChar, -1).Value =
                    string.IsNullOrEmpty(txtAvatarUrl.Text) ? (object)DBNull.Value : txtAvatarUrl.Text;
                cmd.Parameters.Add("@StatusText", SqlDbType.NVarChar, -1).Value =
                    string.IsNullOrEmpty(txtStatusText.Text) ? (object)DBNull.Value : txtStatusText.Text;
                cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = chkIsActive.Checked;

                cmd.ExecuteNonQuery();
            }

            LoadUsers();
            LockControls();
            MessageBox.Show(isAdding ? "✅ Thêm thành công!" : "✏️ Sửa thành công!");
            isAdding = false;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("⚠️ Hãy chọn người dùng cần xóa!");
                return;
            }

            if (!Guid.TryParse(txtId.Text.Trim(), out Guid userId))
            {
                MessageBox.Show("❌ Id không hợp lệ, không thể xóa.");
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM dbo.Users WHERE Id = @Id", conn);
                    cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = userId;
                    cmd.ExecuteNonQuery();
                }

                // Xoá luôn bản lưu mật khẩu cục bộ (nếu có)
                adminSecretsCache.Remove(userId.ToString());
                adminSecretsCache.Remove(userId.ToString() + "_salt");
                SaveAdminSecretsToFile();

                LoadUsers();
                ClearFields();
                MessageBox.Show("🗑️ Đã xóa!");
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
            LockControls();
            dgvUsers.ClearSelection();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Hash SHA256 (lưu vào DB dưới dạng hex string rồi convert -> varbinary ở SQL)
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password ?? string.Empty));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
