using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace btl_lttq.Friendprofile
{
    public partial class ProfileForm : Form
    {
        private readonly string CurrentUsername = "anninh";
        private readonly string _connStr;
        private bool isEditing = false;

        // 🔹 Dữ liệu gốc để khôi phục
        private string originalFullName, originalEmail, originalPhone, originalGender;
        private string originalHometown, originalEducation, originalWork, originalRelationship;

        public ProfileForm()
        {
            InitializeComponent();
            _connStr = ConfigurationManager.ConnectionStrings["MessengerDb"].ConnectionString;
        }

        private void ProfileForm_Load(object sender, EventArgs e)
        {
            this.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            this.BackColor = Color.WhiteSmoke;

            cboGender.Items.Clear();
            cboGender.Items.AddRange(new[] { "Nam", "Nữ", "Khác" });
            cboGender.SelectedIndexChanged += (s, ev) =>
            {
                cboGender.Text = cboGender.SelectedItem?.ToString();
            };

            LoadUserProfile();
            SetEditMode(false);

            StyleButton(btnEdit, Color.FromArgb(66, 133, 244));
            StyleButton(btnUpdate, Color.FromArgb(52, 168, 83));

            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is TextBox txt)
                {
                    txt.ForeColor = Color.FromArgb(0, 102, 255);
                    txt.Font = new Font("Segoe UI", 10, FontStyle.Regular);
                }
            }
        }

        private void StyleButton(Button btn, Color color)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            btn.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btn.Width, btn.Height, 12, 12));

            btn.MouseEnter += (s, e) => btn.BackColor = ControlPaint.Light(color, 0.2f);
            btn.MouseLeave += (s, e) => btn.BackColor = color;
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect,
            int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        // 🔹 Load dữ liệu hồ sơ từ SQL
        private void LoadUserProfile()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connStr))
                {
                    conn.Open();
                    string sql = @"
                        SELECT 
                            u.Email,
                            p.FullName,
                            p.Gender,
                            p.Phone,
                            p.Hometown,
                            p.Education,
                            p.Work,
                            p.Relationship
                        FROM dbo.Users u
                        LEFT JOIN dbo.UserProfiles p ON u.Id = p.UserId
                        WHERE u.UserName = @u";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", CurrentUsername);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtFullName.Text = reader["FullName"]?.ToString();
                                txtEmail.Text = reader["Email"]?.ToString();
                                txtPhone.Text = reader["Phone"]?.ToString();
                                cboGender.Text = reader["Gender"]?.ToString();
                                txtHometown.Text = reader["Hometown"]?.ToString();
                                txtEducation.Text = reader["Education"]?.ToString();
                                txtWork.Text = reader["Work"]?.ToString();
                                txtRelationship.Text = reader["Relationship"]?.ToString();

                                if (string.IsNullOrEmpty(cboGender.Text) && cboGender.Items.Count > 0)
                                    cboGender.SelectedIndex = 0;

                                SaveOriginalValues();
                            }
                            else
                            {
                                MessageBox.Show("Không tìm thấy thông tin hồ sơ người dùng!", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi tải hồ sơ: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 🔹 Lưu dữ liệu gốc
        private void SaveOriginalValues()
        {
            originalFullName = txtFullName.Text;
            originalEmail = txtEmail.Text;
            originalPhone = txtPhone.Text;
            originalGender = cboGender.Text;
            originalHometown = txtHometown.Text;
            originalEducation = txtEducation.Text;
            originalWork = txtWork.Text;
            originalRelationship = txtRelationship.Text;
        }

        // 🔹 Khôi phục dữ liệu gốc
        private void RestoreOriginalValues()
        {
            txtFullName.Text = originalFullName;
            txtEmail.Text = originalEmail;
            txtPhone.Text = originalPhone;
            cboGender.Text = originalGender;
            txtHometown.Text = originalHometown;
            txtEducation.Text = originalEducation;
            txtWork.Text = originalWork;
            txtRelationship.Text = originalRelationship;
        }

        // 🔹 Bật / tắt chế độ chỉnh sửa
        private void SetEditMode(bool enable)
        {
            Color bgColor = Color.FromArgb(245, 245, 245);
            Color textColor = Color.FromArgb(30, 30, 30);

            var boxes = new[]
            {
                txtFullName, txtEmail, txtPhone,
                txtHometown, txtEducation, txtWork, txtRelationship
            };

            foreach (var box in boxes)
            {
                if (box.Controls.OfType<TextBox>().FirstOrDefault() is TextBox inner)
                {
                    inner.ReadOnly = !enable;
                    inner.BackColor = bgColor;
                    inner.ForeColor = textColor;
                    inner.BorderStyle = BorderStyle.None;
                    inner.Cursor = enable ? Cursors.IBeam : Cursors.Default;
                }

                box.ReadOnly = !enable;
                box.EditingMode = enable;
                box.BackColor = this.BackColor;
                box.TabStop = enable;
            }

            cboGender.Enabled = enable;
            cboGender.BackColor = bgColor;
            cboGender.ForeColor = textColor;
            btnUpdate.Enabled = enable;
        }

        // 🔹 Sửa hoặc hủy
        private void btnEdit_Click(object sender, EventArgs e)
        {
            isEditing = !isEditing;

            if (isEditing)
            {
                btnEdit.Text = "❌ Hủy sửa";
                SetEditMode(true);
            }
            else
            {
                RestoreOriginalValues();
                btnEdit.Text = "✏️ Sửa thông tin";
                SetEditMode(false);
            }
        }

        // 🔹 Cập nhật hồ sơ
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!isEditing)
            {
                MessageBox.Show("Hãy nhấn 'Sửa thông tin' trước khi cập nhật!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(_connStr))
                {
                    conn.Open();

                    string sql = @"
                        UPDATE u
                        SET u.Email = @em, u.UpdatedAt = SYSUTCDATETIME()
                        FROM dbo.Users u
                        WHERE u.UserName = @u;

                        UPDATE p
                        SET 
                            p.FullName = @n,
                            p.Phone = @p,
                            p.Gender = @g,
                            p.Hometown = @h,
                            p.Education = @e,
                            p.Work = @w,
                            p.Relationship = @r,
                            p.UpdatedAt = SYSUTCDATETIME()
                        FROM dbo.UserProfiles p
                        INNER JOIN dbo.Users u ON p.UserId = u.Id
                        WHERE u.UserName = @u;";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@em", txtEmail.Text);
                        cmd.Parameters.AddWithValue("@n", txtFullName.Text);
                        cmd.Parameters.AddWithValue("@p", txtPhone.Text);
                        cmd.Parameters.AddWithValue("@g", cboGender.Text);
                        cmd.Parameters.AddWithValue("@h", txtHometown.Text);
                        cmd.Parameters.AddWithValue("@e", txtEducation.Text);
                        cmd.Parameters.AddWithValue("@w", txtWork.Text);
                        cmd.Parameters.AddWithValue("@r", txtRelationship.Text);
                        cmd.Parameters.AddWithValue("@u", CurrentUsername);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("✅ Cập nhật hồ sơ thành công!", "Thành công", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                SaveOriginalValues();
                SetEditMode(false);
                isEditing = false;
                btnEdit.Text = "✏️ Sửa thông tin";
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi cập nhật: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
