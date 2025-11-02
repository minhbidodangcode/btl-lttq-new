namespace btl_lttq.Admin
{
    partial class FormDoanChat
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lvGroups = new System.Windows.Forms.ListView();
            this.contextGroups = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.thêmNhómToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xóaNhómToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lbMembers = new System.Windows.Forms.ListBox();
            this.contextMembers = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.thêmThànhViênToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xóaThànhViênToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.contextGroups.SuspendLayout();
            this.contextMembers.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvGroups
            // 
            this.lvGroups.ContextMenuStrip = this.contextGroups;
            this.lvGroups.FullRowSelect = true;
            this.lvGroups.HideSelection = false;
            this.lvGroups.Location = new System.Drawing.Point(0, 59);
            this.lvGroups.Name = "lvGroups";
            this.lvGroups.Size = new System.Drawing.Size(420, 389);
            this.lvGroups.TabIndex = 0;
            this.lvGroups.UseCompatibleStateImageBehavior = false;
            this.lvGroups.SelectedIndexChanged += new System.EventHandler(this.lvGroups_SelectedIndexChanged);
            // 
            // contextGroups
            // 
            this.contextGroups.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextGroups.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.thêmNhómToolStripMenuItem,
            this.xóaNhómToolStripMenuItem});
            this.contextGroups.Name = "contextGroups";
            this.contextGroups.Size = new System.Drawing.Size(158, 52);
            // 
            // thêmNhómToolStripMenuItem
            // 
            this.thêmNhómToolStripMenuItem.Name = "thêmNhómToolStripMenuItem";
            this.thêmNhómToolStripMenuItem.Size = new System.Drawing.Size(157, 24);
            this.thêmNhómToolStripMenuItem.Text = "Thêm nhóm";
            this.thêmNhómToolStripMenuItem.Click += new System.EventHandler(this.thêmNhómToolStripMenuItem_Click);
            // 
            // xóaNhómToolStripMenuItem
            // 
            this.xóaNhómToolStripMenuItem.Name = "xóaNhómToolStripMenuItem";
            this.xóaNhómToolStripMenuItem.Size = new System.Drawing.Size(157, 24);
            this.xóaNhómToolStripMenuItem.Text = "Xóa nhóm";
            this.xóaNhómToolStripMenuItem.Click += new System.EventHandler(this.xóaNhómToolStripMenuItem_Click);
            // 
            // lbMembers
            // 
            this.lbMembers.ContextMenuStrip = this.contextMembers;
            this.lbMembers.FormattingEnabled = true;
            this.lbMembers.ItemHeight = 16;
            this.lbMembers.Location = new System.Drawing.Point(426, 60);
            this.lbMembers.Name = "lbMembers";
            this.lbMembers.Size = new System.Drawing.Size(406, 388);
            this.lbMembers.TabIndex = 1;
            // 
            // contextMembers
            // 
            this.contextMembers.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMembers.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.thêmThànhViênToolStripMenuItem,
            this.xóaThànhViênToolStripMenuItem});
            this.contextMembers.Name = "contextMembers";
            this.contextMembers.Size = new System.Drawing.Size(188, 52);
            // 
            // thêmThànhViênToolStripMenuItem
            // 
            this.thêmThànhViênToolStripMenuItem.Name = "thêmThànhViênToolStripMenuItem";
            this.thêmThànhViênToolStripMenuItem.Size = new System.Drawing.Size(187, 24);
            this.thêmThànhViênToolStripMenuItem.Text = "Thêm thành viên";
            this.thêmThànhViênToolStripMenuItem.Click += new System.EventHandler(this.thêmThànhViênToolStripMenuItem_Click);
            // 
            // xóaThànhViênToolStripMenuItem
            // 
            this.xóaThànhViênToolStripMenuItem.Name = "xóaThànhViênToolStripMenuItem";
            this.xóaThànhViênToolStripMenuItem.Size = new System.Drawing.Size(187, 24);
            this.xóaThànhViênToolStripMenuItem.Text = "Xóa thành viên";
            this.xóaThànhViênToolStripMenuItem.Click += new System.EventHandler(this.xóaThànhViênToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 19.8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Highlight;
            this.label1.Location = new System.Drawing.Point(274, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(308, 38);
            this.label1.TabIndex = 2;
            this.label1.Text = "Quản lý nhóm chat";
            // 
            // FormDoanChat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.RosyBrown;
            this.ClientSize = new System.Drawing.Size(844, 480);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbMembers);
            this.Controls.Add(this.lvGroups);
            this.Name = "FormDoanChat";
            this.Text = "FormDoanChat";
            this.Load += new System.EventHandler(this.FormDoanChat_Load);
            this.contextGroups.ResumeLayout(false);
            this.contextMembers.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvGroups;
        private System.Windows.Forms.ListBox lbMembers;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip contextGroups;
        private System.Windows.Forms.ContextMenuStrip contextMembers;
        private System.Windows.Forms.ToolStripMenuItem thêmNhómToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem xóaNhómToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem thêmThànhViênToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem xóaThànhViênToolStripMenuItem;
    }
}
