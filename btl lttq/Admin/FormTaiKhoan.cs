using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Text.Json;

namespace btl_lttq.Admin
{
    public partial class FormTaiKhoan : Form
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["MessengerDb"].ConnectionString;

        private bool isAdding = false;

        // Giữ hash gốc của bản ghi đang chọn (để nếu user không nhập pass mới thì giữ nguyên)
        private string currentPasswordHash = null;
        private string currentPasswordSalt = null;

        // File local để lưu bản mã hoá có thể phục hồi cho admin (KHÔNG an toàn cho production)
        private readonly string adminSecretsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "admin_passwords.dat");

        // Cấu trúc lưu file: Dictionary<userId, base64(encrypted bytes)>
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

            LockControls();
        }

        #region Admin secrets file handling (DPAPI)
        // Load file into cache
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
                // Nếu lỗi đọc/parse thì reset cache (không ném exception để khỏi crash form)
                adminSecretsCache = new Dictionary<string, string>();
            }
        }

        // Save cache -> file
        private void SaveAdminSecretsToFile()
        {
            try
            {
                string json = JsonSerializer.Serialize(adminSecretsCache);
                File.WriteAllText(adminSecretsFile, json, Encoding.UTF8);

                // (Optionally) set file attributes to hidden for small protection
                try { File.SetAttributes(adminSecretsFile, FileAttributes.Hidden); } catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không lưu được file admin secrets: " + ex.Message);
            }
        }

        // Mã hoá plainText bằng DPAPI (CurrentUser)
        private string ProtectString(string plain)
        {
            if (string.IsNullOrEmpty(plain)) return string.Empty;
            byte[] plainBytes = Encoding.UTF8.GetBytes(plain);
            byte[] cipher = ProtectedData.Protect(plainBytes, null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(cipher);
        }

        // Giải mã DPAPI
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

        // Lưu mật khẩu thô (plain) cho admin vào file (mã hoá trước)
        private void SavePlainPasswordForAdmin(string userId, string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(userId)) return;
            if (plainPassword == null) plainPassword = string.Empty;
            string protectedStr = ProtectString(plainPassword);
            adminSecretsCache[userId] = protectedStr;
            SaveAdminSecretsToFile();
        }

        // Lấy mật khẩu thô (nếu có) cho userId
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

            // Lưu hash thật vào biến ẩn (dùng nếu user không nhập mật khẩu mới)
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

            // Thử lấy mật khẩu gốc (nếu admin đã lưu nó cục bộ trước đó)
            string userId = txtId.Text;
            string plainPassword = GetPlainPasswordForAdmin(userId);
            txtPasswordHash.Text = plainPassword; // nếu rỗng → textbox trống

            // Salt (tùy bạn có muốn hiển thị)
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

            // 2. Xử lý mật khẩu:
            // Nếu isAdding: bắt buộc nhập mật khẩu
            // Nếu sửa: nếu textbox mật khẩu không rỗng => hash mới; nếu rỗng => giữ hash cũ
            string passwordToSaveHash; // string hex của SHA256
            string saltToSaveHash;

            if (isAdding)
            {
                if (string.IsNullOrWhiteSpace(txtPasswordHash.Text))
                {
                    MessageBox.Show("⚠️ Vui lòng nhập mật khẩu cho người dùng mới!");
                    return;
                }
                // hash mật khẩu để lưu vào DB (không thể khôi phục)
                passwordToSaveHash = HashPassword(txtPasswordHash.Text);
                saltToSaveHash = HashPassword(txtPasswordSalt.Text ?? string.Empty);

                // Đồng thời lưu bản plaintext đã mã hoá cho admin vào file local
                SavePlainPasswordForAdmin(userId.ToString(), txtPasswordHash.Text);
                SavePlainPasswordForAdmin(userId.ToString() + "_salt", txtPasswordSalt.Text ?? string.Empty);
            }
            else
            {
                // Sửa
                if (!string.IsNullOrWhiteSpace(txtPasswordHash.Text))
                {
                    // admin nhập pass mới → hash rồi lưu; đồng thời cập nhật bản lưu cục bộ
                    passwordToSaveHash = HashPassword(txtPasswordHash.Text);
                    SavePlainPasswordForAdmin(userId.ToString(), txtPasswordHash.Text);
                }
                else
                {
                    // không nhập gì -> giữ hash cũ (đã đọc từ DB lúc chọn)
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
