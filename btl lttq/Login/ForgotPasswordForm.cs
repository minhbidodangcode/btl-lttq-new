using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace btl_lttq.Login

{
    public partial class ForgotPasswordForm : Form
    {
        public ForgotPasswordForm()
        {
            InitializeComponent();
        }

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

        private void ForgotPasswordForm_Load(object sender, EventArgs e)
        {
            RoundControl(pnlCard, 20);
            SetPlaceholder(txtEmail, "Nhập email...");
            SetPlaceholder(txtUserName, "Nhập tên người dùng...");
            SetPlaceholder(txtNewPassword, "Nhập mật khẩu mới...", true);

            WireTextboxFocusStyle(txtEmail);
            WireTextboxFocusStyle(txtUserName);
            WireTextboxFocusStyle(txtNewPassword);
        }

        private void btnBack_Click(object sender, EventArgs e) => this.Close();

        private void btnReset_Click(object sender, EventArgs e)
        {
            string email = (txtEmail.ForeColor == Color.Gray) ? "" : txtEmail.Text.Trim();
            string userName = (txtUserName.ForeColor == Color.Gray) ? "" : txtUserName.Text.Trim();
            string newPwd = (txtNewPassword.ForeColor == Color.Gray) ? "" : txtNewPassword.Text.Trim();

            if (email == "" || userName == "" || newPwd == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            try
            {
                var (ok, msg) = AuthService.ResetPassword(email, userName, newPwd);
                MessageBox.Show(msg);
                if (ok) this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi SQL: " + ex.Message);
            }
        }

        // Helpers
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

        private System.Drawing.Drawing2D.GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int d = radius * 2;
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

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
