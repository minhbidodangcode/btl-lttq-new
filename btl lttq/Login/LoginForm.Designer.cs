using System.Drawing;
using System.Windows.Forms;

namespace btl_lttq.Login

{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel pnlCard;
        private Label lblLogo;
        private Label lblTitle;
        private Label lblEmail;
        private TextBox txtEmail;
        private Label lblPassword;
        private TextBox txtPassword;
        private Button btnLogin;
        private LinkLabel linkRegister;
        private LinkLabel linkForgot;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlCard = new System.Windows.Forms.Panel();
            this.lblLogo = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblEmail = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.linkRegister = new System.Windows.Forms.LinkLabel();
            this.linkForgot = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // Form
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = Color.White;
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Đăng nhập";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.Paint += new PaintEventHandler(this.Form_PaintGradient);
            // Card
            this.pnlCard.BackColor = Color.White;
            this.pnlCard.Location = new System.Drawing.Point(190, 90);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Size = new System.Drawing.Size(520, 420);
            this.pnlCard.Anchor = AnchorStyles.None;
            // Logo
            this.lblLogo.AutoSize = false;
            this.lblLogo.Text = "💬  Messenger";
            this.lblLogo.TextAlign = ContentAlignment.MiddleCenter;
            this.lblLogo.Font = new Font("Segoe UI Semibold", 22F, FontStyle.Bold);
            this.lblLogo.ForeColor = Color.FromArgb(20, 120, 140);
            this.lblLogo.Location = new Point(0, 28);
            this.lblLogo.Size = new Size(520, 50);
            // Title
            this.lblTitle.AutoSize = false;
            this.lblTitle.Text = "Welcome back";
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            this.lblTitle.Font = new Font("Segoe UI", 13F);
            this.lblTitle.ForeColor = Color.FromArgb(70, 90, 100);
            this.lblTitle.Location = new Point(0, 78);
            this.lblTitle.Size = new Size(520, 30);
            // Email
            this.lblEmail.AutoSize = true;
            this.lblEmail.Text = "Email";
            this.lblEmail.ForeColor = Color.FromArgb(60, 80, 90);
            this.lblEmail.Location = new Point(90, 130);
            // txtEmail
            this.txtEmail.Location = new Point(90, 155);
            this.txtEmail.Size = new Size(340, 30);
            this.txtEmail.BorderStyle = BorderStyle.FixedSingle;
            // Password
            this.lblPassword.AutoSize = true;
            this.lblPassword.Text = "Mật khẩu";
            this.lblPassword.ForeColor = Color.FromArgb(60, 80, 90);
            this.lblPassword.Location = new Point(90, 205);
            // txtPassword
            this.txtPassword.Location = new Point(90, 230);
            this.txtPassword.Size = new Size(340, 30);
            this.txtPassword.PasswordChar = '•';
            this.txtPassword.BorderStyle = BorderStyle.FixedSingle;
            // btnLogin
            this.btnLogin.Text = "Đăng nhập";
            this.btnLogin.Location = new Point(90, 285);
            this.btnLogin.Size = new Size(340, 40);
            this.btnLogin.BackColor = Color.FromArgb(0, 153, 188);
            this.btnLogin.ForeColor = Color.White;
            this.btnLogin.FlatStyle = FlatStyle.Flat;
            this.btnLogin.FlatAppearance.BorderSize = 0;
            this.btnLogin.Cursor = Cursors.Hand;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // linkRegister
            this.linkRegister.AutoSize = true;
            this.linkRegister.Text = "Chưa có tài khoản? Đăng ký";
            this.linkRegister.Location = new Point(90, 340);
            this.linkRegister.LinkColor = Color.FromArgb(0, 120, 160);
            this.linkRegister.ActiveLinkColor = Color.FromArgb(0, 153, 188);
            this.linkRegister.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkRegister_LinkClicked);
            // linkForgot
            this.linkForgot.AutoSize = true;
            this.linkForgot.Text = "Quên mật khẩu?";
            this.linkForgot.Location = new Point(90, 367);
            this.linkForgot.LinkColor = Color.FromArgb(0, 120, 160);
            this.linkForgot.ActiveLinkColor = Color.FromArgb(0, 153, 188);
            this.linkForgot.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkForgot_LinkClicked);

            // add controls
            this.pnlCard.Controls.Add(this.lblLogo);
            this.pnlCard.Controls.Add(this.lblTitle);
            this.pnlCard.Controls.Add(this.lblEmail);
            this.pnlCard.Controls.Add(this.txtEmail);
            this.pnlCard.Controls.Add(this.lblPassword);
            this.pnlCard.Controls.Add(this.txtPassword);
            this.pnlCard.Controls.Add(this.btnLogin);
            this.pnlCard.Controls.Add(this.linkRegister);
            this.pnlCard.Controls.Add(this.linkForgot);
            this.Controls.Add(this.pnlCard);

            this.ResumeLayout(false);
        }
    }
}
