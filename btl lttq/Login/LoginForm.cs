using btl_lttq.ChatClient;
using btl_lttq.FacebookLite;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace btl_lttq.Login
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        // 🎨 Gradient nền
        private void Form_PaintGradient(object sender, PaintEventArgs e)
        {
            using (var br = new LinearGradientBrush(this.ClientRectangle,
                                                    Color.FromArgb(0, 180, 219),
                                                    Color.FromArgb(0, 131, 176),
                                                    LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(br, this.ClientRectangle);
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            RoundControl(pnlCard, 20);
            SetPlaceholder(txtEmail, "Nhập email...");
            SetPlaceholder(txtPassword, "Nhập mật khẩu...", true);

            WireTextboxFocusStyle(txtEmail);
            WireTextboxFocusStyle(txtPassword);
        }

        // 🟦 Bo tròn panel
        private void RoundControl(Control c, int radius)
        {
            c.Paint += (s, e) =>
            {
                using (var path = RoundedRect(new Rectangle(Point.Empty, c.Size), radius))
                using (var pen = new Pen(Color.FromArgb(230, 230, 230), 1))
                {
                    c.Region = new Region(path);
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.DrawPath(pen, path);
                }
            };
            c.Invalidate();
        }

        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        // ✏️ Placeholder TextBox
        private void SetPlaceholder(TextBox tb, string text, bool isPassword = false)
        {
            tb.Tag = new Tuple<string, bool>(text, isPassword);
            tb.ForeColor = Color.Gray;
            tb.Text = text;
            if (isPassword) tb.PasswordChar = '\0';

            tb.GotFocus += (s, e) =>
            {
                var t = (Tuple<string, bool>)tb.Tag;
                if (tb.Text == t.Item1)
                {
                    tb.Text = "";
                    tb.ForeColor = Color.Black;
                    if (t.Item2) tb.PasswordChar = '•';
                }
            };
            tb.LostFocus += (s, e) =>
            {
                var t = (Tuple<string, bool>)tb.Tag;
                if (string.IsNullOrWhiteSpace(tb.Text))
                {
                    tb.ForeColor = Color.Gray;
                    tb.Text = t.Item1;
                    if (t.Item2) tb.PasswordChar = '\0';
                }
            };
        }

        // 🌈 Viền xanh khi focus
        private void WireTextboxFocusStyle(TextBox tb)
        {
            Color normal = Color.FromArgb(220, 220, 220);
            Color focus = Color.FromArgb(0, 153, 188);

            tb.Parent.Paint += (s, e) =>
            {
                var rect = tb.Bounds; rect.Inflate(1, 1);
                using (var pen = new Pen(tb.Focused ? focus : normal, 1))
                {
                    e.Graphics.DrawRectangle(pen, rect);
                }
            };
            tb.TextChanged += (s, e) => tb.Parent.Invalidate();
            tb.Enter += (s, e) => tb.Parent.Invalidate();
            tb.Leave += (s, e) => tb.Parent.Invalidate();
        }

        // 🔑 Nút Đăng nhập
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string email = (txtEmail.ForeColor == Color.Gray) ? "" : txtEmail.Text.Trim();
            string password = (txtPassword.ForeColor == Color.Gray) ? "" : txtPassword.Text.Trim();

            if (email == "" || password == "")
            {
                MessageBox.Show("Vui lòng nhập Email và Mật khẩu!", "Thiếu thông tin");
                return;
            }

            // ✅ Tài khoản admin
            if (email.Equals("admin", StringComparison.OrdinalIgnoreCase)
                && password == "88888888")
            {
                var menu = new Admin.FormMenu();
                this.Hide();
                menu.ShowDialog();
                this.Close();
                return;
            }

            // ✅ Tài khoản người dùng thường
            try
            {
                if (AuthService.Login(email, password))
                {
                    // Lấy UserId
                    Guid userId = DatabaseHelper.GetUserIdByEmailAndPassword(email, password);
                    if (userId == Guid.Empty)
                    {
                        MessageBox.Show("Không tìm thấy UserId. Kiểm tra lại bảng Users!", "Lỗi");
                        return;
                    }

                    MessageBox.Show("Đăng nhập thành công!", "Thành công");

                    // Mở MessengerForm, truyền userId và email
                    MessengerForm messengerForm = new MessengerForm(userId, email);
                    this.Hide();
                    messengerForm.ShowDialog();

                    this.Close();
                }
                else
                {
                    MessageBox.Show("Sai Email hoặc Mật khẩu!", "Lỗi");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message);
            }
        }

        // 🪪 Mở form Đăng ký
        private void linkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            var frm = new RegisterForm();
            frm.FormClosed += (s, args) =>
            {
                ResetLoginForm();
                this.Show();
            };
            frm.Show();
        }

        // 🧩 Mở form Quên mật khẩu
        private void linkForgot_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            var frm = new ForgotPasswordForm();
            frm.FormClosed += (s, args) =>
            {
                ResetLoginForm();
                this.Show();
            };
            frm.Show();
        }

        // 🧹 Reset nội dung form đăng nhập
        private void ResetLoginForm()
        {
            txtEmail.ForeColor = Color.Gray;
            txtEmail.Text = "Nhập email...";
            txtPassword.ForeColor = Color.Gray;
            txtPassword.Text = "Nhập mật khẩu...";
            txtPassword.PasswordChar = '\0';
        }
    }
}
