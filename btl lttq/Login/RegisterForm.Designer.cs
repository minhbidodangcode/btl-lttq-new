using System.Drawing;
using System.Windows.Forms;

namespace btl_lttq.Login

{
    partial class RegisterForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel pnlCard;
        private Button btnBack;
        private Label lblLogo;
        private Label lblTitle;
        private Label lblEmail;
        private TextBox txtEmail;
        private Label lblUserName;
        private TextBox txtUserName;
        private Label lblPassword;
        private TextBox txtPassword;
        private Button btnRegister;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlCard = new Panel();
            this.btnBack = new Button();
            this.lblLogo = new Label();
            this.lblTitle = new Label();
            this.lblEmail = new Label();
            this.txtEmail = new TextBox();
            this.lblUserName = new Label();
            this.txtUserName = new TextBox();
            this.lblPassword = new Label();
            this.txtPassword = new TextBox();
            this.btnRegister = new Button();
            this.SuspendLayout();

            // Form
            this.AutoScaleMode = AutoScaleMode.None;
            this.BackColor = Color.White;
            this.ClientSize = new Size(900, 600);
            this.DoubleBuffered = true;
            this.Font = new Font("Segoe UI", 10F);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Name = "RegisterForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Đăng ký";
            this.Load += new System.EventHandler(this.RegisterForm_Load);
            this.Paint += new PaintEventHandler(this.Form_PaintGradient);

            // Card
            this.pnlCard.BackColor = Color.White;
            this.pnlCard.Location = new Point(190, 90);
            this.pnlCard.Size = new Size(520, 420);

            // Back
            this.btnBack.Text = "← Quay lại";
            this.btnBack.Location = new Point(16, 14);
            this.btnBack.Size = new Size(96, 32);
            this.btnBack.BackColor = Color.FromArgb(0, 153, 188);
            this.btnBack.ForeColor = Color.White;
            this.btnBack.FlatStyle = FlatStyle.Flat;
            this.btnBack.FlatAppearance.BorderSize = 0;
            this.btnBack.Cursor = Cursors.Hand;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);

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
            this.lblTitle.Text = "Create your account";
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            this.lblTitle.Font = new Font("Segoe UI", 13F);
            this.lblTitle.ForeColor = Color.FromArgb(70, 90, 100);
            this.lblTitle.Location = new Point(0, 78);
            this.lblTitle.Size = new Size(520, 30);

            // Email
            this.lblEmail.AutoSize = true;
            this.lblEmail.Text = "Email *";
            this.lblEmail.ForeColor = Color.FromArgb(60, 80, 90);
            this.lblEmail.Location = new Point(90, 130);

            this.txtEmail.Location = new Point(90, 155);
            this.txtEmail.Size = new Size(340, 30);
            this.txtEmail.BorderStyle = BorderStyle.FixedSingle;

            // UserName
            this.lblUserName.AutoSize = true;
            this.lblUserName.Text = "Tên người dùng *";
            this.lblUserName.ForeColor = Color.FromArgb(60, 80, 90);
            this.lblUserName.Location = new Point(90, 205);

            this.txtUserName.Location = new Point(90, 230);
            this.txtUserName.Size = new Size(340, 30);
            this.txtUserName.BorderStyle = BorderStyle.FixedSingle;

            // Password
            this.lblPassword.AutoSize = true;
            this.lblPassword.Text = "Mật khẩu *";
            this.lblPassword.ForeColor = Color.FromArgb(60, 80, 90);
            this.lblPassword.Location = new Point(90, 280);

            this.txtPassword.Location = new Point(90, 305);
            this.txtPassword.Size = new Size(340, 30);
            this.txtPassword.PasswordChar = '•';
            this.txtPassword.BorderStyle = BorderStyle.FixedSingle;

            // Register button
            this.btnRegister.Text = "Đăng ký";
            this.btnRegister.Location = new Point(90, 360);
            this.btnRegister.Size = new Size(340, 40);
            this.btnRegister.BackColor = Color.FromArgb(0, 153, 188);
            this.btnRegister.ForeColor = Color.White;
            this.btnRegister.FlatStyle = FlatStyle.Flat;
            this.btnRegister.FlatAppearance.BorderSize = 0;
            this.btnRegister.Cursor = Cursors.Hand;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);

            // Add to card
            this.pnlCard.Controls.Add(this.lblLogo);
            this.pnlCard.Controls.Add(this.lblTitle);
            this.pnlCard.Controls.Add(this.lblEmail);
            this.pnlCard.Controls.Add(this.txtEmail);
            this.pnlCard.Controls.Add(this.lblUserName);
            this.pnlCard.Controls.Add(this.txtUserName);
            this.pnlCard.Controls.Add(this.lblPassword);
            this.pnlCard.Controls.Add(this.txtPassword);
            this.pnlCard.Controls.Add(this.btnRegister);

            // Add to form
            this.Controls.Add(this.pnlCard);
            this.Controls.Add(this.btnBack);

            this.ResumeLayout(false);
        }
    }
}
