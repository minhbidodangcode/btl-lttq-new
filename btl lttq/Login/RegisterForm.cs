using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace btl_lttq.Login
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        // 🩵 Gradient nền
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

        private void RegisterForm_Load(object sender, EventArgs e)
        {
            RoundControl(pnlCard, 20);
            SetPlaceholder(txtEmail, "Nhập email...");
            SetPlaceholder(txtUserName, "Nhập tên người dùng...");
            SetPlaceholder(txtPassword, "Nhập mật khẩu...", true);

            WireTextboxFocusStyle(txtEmail);
            WireTextboxFocusStyle(txtUserName);
            WireTextboxFocusStyle(txtPassword);
        }

        private void btnBack_Click(object sender, EventArgs e) => this.Close();

        // 🧩 Nút Đăng ký
        private void btnRegister_Click(object sender, EventArgs e)
        {
            string email = (txtEmail.ForeColor == Color.Gray) ? "" : txtEmail.Text.Trim();
            string userName = (txtUserName.ForeColor == Color.Gray) ? "" : txtUserName.Text.Trim();
            string password = (txtPassword.ForeColor == Color.Gray) ? "" : txtPassword.Text.Trim();

            if (email == "" || userName == "" || password == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Email, Tên người dùng và Mật khẩu!", "Thiếu thông tin");
                return;
            }

            try
            {
                var (ok, msg) = AuthService.Register(email, userName, password);
                MessageBox.Show(msg, ok ? "Thành công" : "Lỗi");
                if (ok) this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi SQL: " + ex.Message);
            }
        }

        // 🌈 Bo tròn khung
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

        // ✏️ Placeholder
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

        // 💡 Viền xanh khi focus
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
    }
}
