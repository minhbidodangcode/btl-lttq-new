using System;
using System.Drawing;
using System.Windows.Forms;

namespace btl_lttq.ChatClient
{
    partial class MessengerForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel panelLeft;
        private Panel panelLeftTop;
        private TextBox txtGlobalSearch;
        private Button btnCreateGroup;
        private ListView lvConversations;
        private Panel panelLeftBottom;
        private Button btnFriends;
        private Button btnProfile;
        private Button btnSettings;

        private Splitter splitterLeft;

        private Panel panelCenter;
        private Panel headerPanel;
        private PictureBox picAvatar;
        private Label lblName;
        private Label lblStatus;
        private TextBox txtSearchInChat;
        private Button btnCall;
        private Button btnVideo;
        private Button btnInfo;

        private Panel panelMessages;
        private Panel inputPanel;
        private TextBox txtMessage;
        private Button btnEmoji;
        private Button btnFile;
        private Button btnSend;

        private Splitter splitterRight;

        private Panel panelRight;
        private Label lblInfoTitle;
        private ComboBox cboBackground;
        private Label lblParticipants;
        private ListBox lstMembers;
        private Label lblSharedFiles;
        private ListView lvSharedFiles;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelLeft = new System.Windows.Forms.Panel();
            this.lvConversations = new System.Windows.Forms.ListView();
            this.panelLeftBottom = new System.Windows.Forms.Panel();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnProfile = new System.Windows.Forms.Button();
            this.btnFriends = new System.Windows.Forms.Button();
            this.panelLeftTop = new System.Windows.Forms.Panel();
            this.btnCreateGroup = new System.Windows.Forms.Button();
            this.txtGlobalSearch = new System.Windows.Forms.TextBox();
            this.splitterLeft = new System.Windows.Forms.Splitter();
            this.panelCenter = new System.Windows.Forms.Panel();
            this.panelMessages = new System.Windows.Forms.Panel();
            this.inputPanel = new System.Windows.Forms.Panel();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnFile = new System.Windows.Forms.Button();
            this.btnEmoji = new System.Windows.Forms.Button();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.picAvatar = new System.Windows.Forms.PictureBox();
            this.lblName = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.txtSearchInChat = new System.Windows.Forms.TextBox();
            this.btnCall = new System.Windows.Forms.Button();
            this.btnVideo = new System.Windows.Forms.Button();
            this.btnInfo = new System.Windows.Forms.Button();
            this.splitterRight = new System.Windows.Forms.Splitter();
            this.panelRight = new System.Windows.Forms.Panel();
            this.lblTheme = new System.Windows.Forms.Label();
            this.lvSharedFiles = new System.Windows.Forms.ListView();
            this.lblSharedFiles = new System.Windows.Forms.Label();
            this.lstMembers = new System.Windows.Forms.ListBox();
            this.lblParticipants = new System.Windows.Forms.Label();
            this.lblInfoTitle = new System.Windows.Forms.Label();
            this.cboBackground = new System.Windows.Forms.ComboBox();
            this.panelLeft.SuspendLayout();
            this.panelLeftBottom.SuspendLayout();
            this.panelLeftTop.SuspendLayout();
            this.panelCenter.SuspendLayout();
            this.inputPanel.SuspendLayout();
            this.headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAvatar)).BeginInit();
            this.panelRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelLeft
            // 
            this.panelLeft.Padding = new Padding(0);
            this.panelLeft.Margin = new Padding(6);
            this.panelLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(248)))));
            this.panelLeft.Controls.Add(this.lvConversations);
            this.panelLeft.Controls.Add(this.panelLeftBottom);
            this.panelLeft.Controls.Add(this.panelLeftTop);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(280, 720);
            this.panelLeft.TabIndex = 4;
            // 
            // lvConversations
            // 
            this.lvConversations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvConversations.FullRowSelect = true;
            this.lvConversations.HideSelection = false;
            this.lvConversations.Location = new System.Drawing.Point(0, 50);
            this.lvConversations.MultiSelect = false;
            this.lvConversations.Name = "lvConversations";
            this.lvConversations.Size = new System.Drawing.Size(280, 614);
            this.lvConversations.TabIndex = 0;
            this.lvConversations.TileSize = new System.Drawing.Size(250, 56);
            this.lvConversations.UseCompatibleStateImageBehavior = false;
            this.lvConversations.View = System.Windows.Forms.View.Tile;
            // 
            // panelLeftBottom
            // 
            this.panelLeftBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(35)))), ((int)(((byte)(37)))));
            this.panelLeftBottom.Controls.Add(this.btnSettings);
            this.panelLeftBottom.Controls.Add(this.btnProfile);
            this.panelLeftBottom.Controls.Add(this.btnFriends);
            this.panelLeftBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelLeftBottom.Location = new System.Drawing.Point(0, 664);
            this.panelLeftBottom.Name = "panelLeftBottom";
            this.panelLeftBottom.Padding = new System.Windows.Forms.Padding(4);
            this.panelLeftBottom.Size = new System.Drawing.Size(280, 56);
            this.panelLeftBottom.TabIndex = 1;
            // 
            // btnSettings
            // 
            this.btnSettings.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnSettings.FlatAppearance.BorderSize = 0;
            this.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSettings.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnSettings.ForeColor = System.Drawing.Color.White;
            this.btnSettings.Location = new System.Drawing.Point(120, 4);
            this.btnSettings.Margin = new System.Windows.Forms.Padding(0);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(56, 48);
            this.btnSettings.TabIndex = 2;
            this.btnSettings.Text = "⚙";
            this.btnSettings.UseVisualStyleBackColor = false;
            // 
            // btnProfile
            // 
            this.btnProfile.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnProfile.FlatAppearance.BorderSize = 0;
            this.btnProfile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProfile.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnProfile.ForeColor = System.Drawing.Color.White;
            this.btnProfile.Location = new System.Drawing.Point(64, 4);
            this.btnProfile.Margin = new System.Windows.Forms.Padding(0);
            this.btnProfile.Name = "btnProfile";
            this.btnProfile.Size = new System.Drawing.Size(56, 48);
            this.btnProfile.TabIndex = 1;
            this.btnProfile.Text = "👤";
            this.btnProfile.UseVisualStyleBackColor = false;
            // 
            // btnFriends
            // 
            this.btnFriends.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnFriends.FlatAppearance.BorderSize = 0;
            this.btnFriends.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFriends.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnFriends.ForeColor = System.Drawing.Color.White;
            this.btnFriends.Location = new System.Drawing.Point(4, 4);
            this.btnFriends.Margin = new System.Windows.Forms.Padding(0);
            this.btnFriends.Name = "btnFriends";
            this.btnFriends.Size = new System.Drawing.Size(60, 48);
            this.btnFriends.TabIndex = 0;
            this.btnFriends.Text = "👥";
            this.btnFriends.UseVisualStyleBackColor = false;
            // 
            // panelLeftTop
            // 
            this.panelLeftTop.BackColor = System.Drawing.Color.White;
            this.panelLeftTop.Controls.Add(this.btnCreateGroup);
            this.panelLeftTop.Controls.Add(this.txtGlobalSearch);
            this.panelLeftTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelLeftTop.Location = new System.Drawing.Point(0, 0);
            this.panelLeftTop.Name = "panelLeftTop";
            this.panelLeftTop.Padding = new System.Windows.Forms.Padding(8);
            this.panelLeftTop.Size = new System.Drawing.Size(280, 50);
            this.panelLeftTop.TabIndex = 2;
            // 
            // btnCreateGroup
            // 
            this.btnCreateGroup.Location = new System.Drawing.Point(198, 17);
            this.btnCreateGroup.Name = "btnCreateGroup";
            this.btnCreateGroup.Size = new System.Drawing.Size(24, 23);
            this.btnCreateGroup.TabIndex = 1;
            this.btnCreateGroup.Text = "+";
            this.btnCreateGroup.UseVisualStyleBackColor = true;
            // 
            // txtGlobalSearch
            // 
            this.txtGlobalSearch.Location = new System.Drawing.Point(12, 18);
            this.txtGlobalSearch.Name = "txtGlobalSearch";
            this.txtGlobalSearch.Size = new System.Drawing.Size(180, 20);
            this.txtGlobalSearch.TabIndex = 0;
            // 
            // splitterLeft
            // 
            this.splitterLeft.BackColor = System.Drawing.Color.Gainsboro;
            this.splitterLeft.Location = new System.Drawing.Point(280, 0);
            this.splitterLeft.Name = "splitterLeft";
            this.splitterLeft.Size = new System.Drawing.Size(2, 720);
            this.splitterLeft.TabIndex = 3;
            this.splitterLeft.TabStop = false;
            // 
            // panelCenter
            // 
            this.panelCenter.BackColor = System.Drawing.Color.White;
            this.panelCenter.Margin = new Padding(6);
            this.panelCenter.Controls.Add(this.panelMessages);
            this.panelCenter.Controls.Add(this.inputPanel);
            this.panelCenter.Controls.Add(this.headerPanel);
            this.panelCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCenter.Location = new System.Drawing.Point(282, 0);
            this.panelCenter.Name = "panelCenter";
            this.panelCenter.Size = new System.Drawing.Size(785, 720);
            this.panelCenter.TabIndex = 0;
            // 
            // panelMessages
            // 
            this.panelMessages.AutoScroll = true;
            this.panelMessages.BackColor = System.Drawing.Color.White;
            this.panelMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMessages.Location = new System.Drawing.Point(0, 64);
            this.panelMessages.Name = "panelMessages";
            this.panelMessages.Size = new System.Drawing.Size(785, 586);
            this.panelMessages.TabIndex = 0;
            // 
            // inputPanel
            // 
            this.inputPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(248)))));
            this.inputPanel.Controls.Add(this.txtMessage);
            this.inputPanel.Controls.Add(this.btnSend);
            this.inputPanel.Controls.Add(this.btnFile);
            this.inputPanel.Controls.Add(this.btnEmoji);
            this.inputPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.inputPanel.Location = new System.Drawing.Point(0, 650);
            this.inputPanel.Name = "inputPanel";
            this.inputPanel.Size = new System.Drawing.Size(785, 70);
            this.inputPanel.TabIndex = 1;
            // 
            // txtMessage
            // 
            this.txtMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMessage.Location = new System.Drawing.Point(68, 0);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessage.Size = new System.Drawing.Size(657, 70);
            this.txtMessage.TabIndex = 0;
            // 
            // btnSend
            // 
            this.btnSend.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSend.Location = new System.Drawing.Point(725, 0);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(60, 70);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "Gửi";
            // 
            // btnFile
            // 
            this.btnFile.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnFile.Location = new System.Drawing.Point(34, 0);
            this.btnFile.Name = "btnFile";
            this.btnFile.Size = new System.Drawing.Size(34, 70);
            this.btnFile.TabIndex = 2;
            this.btnFile.Text = "📎";
            // 
            // btnEmoji
            // 
            this.btnEmoji.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnEmoji.Location = new System.Drawing.Point(0, 0);
            this.btnEmoji.Name = "btnEmoji";
            this.btnEmoji.Size = new System.Drawing.Size(34, 70);
            this.btnEmoji.TabIndex = 3;
            this.btnEmoji.Text = "😊";
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.White;
            this.headerPanel.Controls.Add(this.picAvatar);
            this.headerPanel.Controls.Add(this.lblName);
            this.headerPanel.Controls.Add(this.lblStatus);
            this.headerPanel.Controls.Add(this.txtSearchInChat);
            this.headerPanel.Controls.Add(this.btnCall);
            this.headerPanel.Controls.Add(this.btnVideo);
            this.headerPanel.Controls.Add(this.btnInfo);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(785, 64);
            this.headerPanel.TabIndex = 2;
            // 
            // picAvatar
            // 
            this.picAvatar.Location = new System.Drawing.Point(12, 12);
            this.picAvatar.Name = "picAvatar";
            this.picAvatar.Size = new System.Drawing.Size(40, 40);
            this.picAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picAvatar.TabIndex = 0;
            this.picAvatar.TabStop = false;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            this.lblName.Location = new System.Drawing.Point(60, 10);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(147, 20);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "Tên cuộc trò chuyện";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.DimGray;
            this.lblStatus.Location = new System.Drawing.Point(60, 30);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(35, 13);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "online";
            // 
            // txtSearchInChat
            // 
            this.txtSearchInChat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearchInChat.Location = new System.Drawing.Point(427, 20);
            this.txtSearchInChat.Name = "txtSearchInChat";
            this.txtSearchInChat.Size = new System.Drawing.Size(160, 20);
            this.txtSearchInChat.TabIndex = 3;
            // 
            // btnCall
            // 
            this.btnCall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCall.Location = new System.Drawing.Point(593, 18);
            this.btnCall.Name = "btnCall";
            this.btnCall.Size = new System.Drawing.Size(32, 28);
            this.btnCall.TabIndex = 4;
            this.btnCall.Text = "📞";
            // 
            // btnVideo
            // 
            this.btnVideo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnVideo.Location = new System.Drawing.Point(631, 18);
            this.btnVideo.Name = "btnVideo";
            this.btnVideo.Size = new System.Drawing.Size(32, 28);
            this.btnVideo.TabIndex = 5;
            this.btnVideo.Text = "🎥";
            // 
            // btnInfo
            // 
            this.btnInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInfo.Location = new System.Drawing.Point(669, 18);
            this.btnInfo.Name = "btnInfo";
            this.btnInfo.Size = new System.Drawing.Size(32, 28);
            this.btnInfo.TabIndex = 6;
            this.btnInfo.Text = "<";
            // 
            // splitterRight
            // 
            this.splitterRight.BackColor = System.Drawing.Color.Gainsboro;
            this.splitterRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitterRight.Location = new System.Drawing.Point(1067, 0);
            this.splitterRight.Name = "splitterRight";
            this.splitterRight.Size = new System.Drawing.Size(2, 720);
            this.splitterRight.TabIndex = 1;
            this.splitterRight.TabStop = false;
            this.splitterRight.Visible = false;
            // 
            // panelRight
            // 
            this.panelRight.BackColor = System.Drawing.Color.White;
            this.panelRight.Margin = new Padding(6);
            this.panelRight.Controls.Add(this.lblTheme);
            this.panelRight.Controls.Add(this.lvSharedFiles);
            this.panelRight.Controls.Add(this.lblSharedFiles);
            this.panelRight.Controls.Add(this.lstMembers);
            this.panelRight.Controls.Add(this.lblParticipants);
            this.panelRight.Controls.Add(this.lblInfoTitle);
            this.panelRight.Controls.Add(this.cboBackground);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Location = new System.Drawing.Point(1069, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Padding = new System.Windows.Forms.Padding(8);
            this.panelRight.Size = new System.Drawing.Size(258, 720);
            this.panelRight.TabIndex = 2;
            this.panelRight.Visible = false;
            // 
            // lblTheme
            // 
            this.lblTheme.AutoSize = true;
            this.lblTheme.Location = new System.Drawing.Point(6, 34);
            this.lblTheme.Name = "lblTheme";
            this.lblTheme.Size = new System.Drawing.Size(40, 13);
            this.lblTheme.TabIndex = 5;
            this.lblTheme.Text = "Theme";
            // 
            // lvSharedFiles
            // 
            this.lvSharedFiles.FullRowSelect = true;
            this.lvSharedFiles.HideSelection = false;
            this.lvSharedFiles.Location = new System.Drawing.Point(8, 338);
            this.lvSharedFiles.Name = "lvSharedFiles";
            this.lvSharedFiles.Size = new System.Drawing.Size(240, 368);
            this.lvSharedFiles.TabIndex = 0;
            this.lvSharedFiles.UseCompatibleStateImageBehavior = false;
            this.lvSharedFiles.View = System.Windows.Forms.View.Details;
            // 
            // lblSharedFiles
            // 
            this.lblSharedFiles.AutoSize = true;
            this.lblSharedFiles.Location = new System.Drawing.Point(4, 308);
            this.lblSharedFiles.Name = "lblSharedFiles";
            this.lblSharedFiles.Size = new System.Drawing.Size(79, 13);
            this.lblSharedFiles.TabIndex = 1;
            this.lblSharedFiles.Text = "Tệp đã chia sẻ";
            // 
            // lstMembers
            // 
            this.lstMembers.Location = new System.Drawing.Point(8, 136);
            this.lstMembers.Name = "lstMembers";
            this.lstMembers.Size = new System.Drawing.Size(240, 147);
            this.lstMembers.TabIndex = 2;
            // 
            // lblParticipants
            // 
            this.lblParticipants.AutoSize = true;
            this.lblParticipants.Location = new System.Drawing.Point(8, 120);
            this.lblParticipants.Name = "lblParticipants";
            this.lblParticipants.Size = new System.Drawing.Size(76, 13);
            this.lblParticipants.TabIndex = 3;
            this.lblParticipants.Text = "Thành viên (0)";
            // 
            // lblInfoTitle
            // 
            this.lblInfoTitle.AutoSize = true;
            this.lblInfoTitle.Location = new System.Drawing.Point(8, 8);
            this.lblInfoTitle.Name = "lblInfoTitle";
            this.lblInfoTitle.Size = new System.Drawing.Size(95, 13);
            this.lblInfoTitle.TabIndex = 4;
            this.lblInfoTitle.Text = "Thông tin hội thoại";
            // 
            // cboBackground
            // 
            this.cboBackground.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBackground.FormattingEnabled = true;
            this.cboBackground.Location = new System.Drawing.Point(7, 50);
            this.cboBackground.Name = "cboBackground";
            this.cboBackground.Size = new System.Drawing.Size(121, 21);
            this.cboBackground.TabIndex = 0;
            // 
            // MessengerForm
            // 
            this.ClientSize = new System.Drawing.Size(1327, 720);
            this.Controls.Add(this.panelCenter);
            this.Controls.Add(this.splitterRight);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.splitterLeft);
            this.Controls.Add(this.panelLeft);
            this.Name = "MessengerForm";
            this.Text = "Messenger";
            this.panelLeft.ResumeLayout(false);
            this.panelLeftBottom.ResumeLayout(false);
            this.panelLeftTop.ResumeLayout(false);
            this.panelLeftTop.PerformLayout();
            this.panelCenter.ResumeLayout(false);
            this.inputPanel.ResumeLayout(false);
            this.inputPanel.PerformLayout();
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAvatar)).EndInit();
            this.panelRight.ResumeLayout(false);
            this.panelRight.PerformLayout();
            this.ResumeLayout(false);

        }

        private Label lblTheme;
    }
}
