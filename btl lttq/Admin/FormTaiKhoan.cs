using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace btl_lttq.Admin
{
    public partial class FormTaiKhoan : Form
    {
        private string connectionString =
            ConfigurationManager.ConnectionStrings["MessengerDb"].ConnectionString;

        bool isAdding = false;

        public FormTaiKhoan()
        {
            InitializeComponent();
        }

        private void FormTaiKhoan_Load(object sender, EventArgs e)
        {
            LoadUsers();
            dgvUsers.CellClick += dgvUsers_CellClick;
            dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsers.MultiSelect = false;

            LockControls();
        }

        private void LoadUsers()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                    SELECT 
                        CONVERT(nvarchar(50), Id) AS Id,
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
                    FROM dbo.Users";

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
            txtPasswordHash.Text = row.Cells["PasswordHash"].Value?.ToString();
            txtPasswordSalt.Text = row.Cells["PasswordSalt"].Value?.ToString();
            txtDisplayName.Text = row.Cells["DisplayName"].Value?.ToString();
            txtAvatarUrl.Text = row.Cells["AvatarUrl"].Value?.ToString();
            txtStatusText.Text = row.Cells["StatusText"].Value?.ToString();
            txtCreatedAt.Text = row.Cells["CreatedAt"].Value?.ToString();
            txtUpdatedAt.Text = row.Cells["UpdatedAt"].Value?.ToString();

            chkIsActive.Checked = Convert.ToBoolean(row.Cells["IsActive"].Value);

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
            txtPasswordHash.ReadOnly = false;
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
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd;

                if (isAdding)
                {
                    cmd = new SqlCommand(@"
                    INSERT INTO dbo.Users
                    (Id, UserName, Email, PasswordHash, PasswordSalt, DisplayName, AvatarUrl, StatusText, IsActive, CreatedAt)
                    VALUES (@Id, @UserName, @Email, CONVERT(varbinary(max), @PasswordHash), CONVERT(varbinary(max), @PasswordSalt),
                    @DisplayName, @AvatarUrl, @StatusText, @IsActive, SYSDATETIME())", conn);
                }
                else
                {
                    cmd = new SqlCommand(@"
                    UPDATE dbo.Users SET
                        UserName=@UserName,
                        Email=@Email,
                        PasswordHash=CONVERT(varbinary(max), @PasswordHash),
                        PasswordSalt=CONVERT(varbinary(max), @PasswordSalt),
                        DisplayName=@DisplayName,
                        AvatarUrl=@AvatarUrl,
                        StatusText=@StatusText,
                        IsActive=@IsActive,
                        UpdatedAt=SYSDATETIME()
                    WHERE Id=@Id", conn);
                }

                cmd.Parameters.AddWithValue("@Id", txtId.Text);
                cmd.Parameters.AddWithValue("@UserName", txtUserName.Text);
                cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                cmd.Parameters.AddWithValue("@PasswordHash", txtPasswordHash.Text);
                cmd.Parameters.AddWithValue("@PasswordSalt", txtPasswordSalt.Text);
                cmd.Parameters.AddWithValue("@DisplayName", txtDisplayName.Text);
                cmd.Parameters.AddWithValue("@AvatarUrl", txtAvatarUrl.Text);
                cmd.Parameters.AddWithValue("@StatusText", txtStatusText.Text);
                cmd.Parameters.AddWithValue("@IsActive", chkIsActive.Checked ? 1 : 0);

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

            if (MessageBox.Show("Bạn có chắc muốn xóa?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("DELETE FROM dbo.Users WHERE Id = @Id", conn);
                    cmd.Parameters.AddWithValue("@Id", txtId.Text);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

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
    }
}
