using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using btl_lttq.Login;

namespace btl_lttq.FacebookLite
{
    public partial class SettingForm : Form
    {
        // Lấy chuỗi kết nối từ App.config
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["MessengerDb"].ConnectionString;

        // User đang đăng nhập (truyền từ form gọi)
        private readonly Guid _currentUserId;

        private BackButton btnBack;
        private Label lblTitle;
        private FlowLayoutPanel flow;
        private Panel bottomBar;
        private Panel header;

        private bool _soundOnPreview = true;
        private string _statusPreview = "Active"; // chỉ là preview UI, KHÔNG đụng DB cho đến khi người dùng bấm
        private string _fontFamilyPreview;

        private ComboBox cmbFontFamily;

        private Label lblSoundLeft, lblStatusLeft, lblFontLeft;
        private Label lSndOn, lSndOff, lSttOn, lSttOff;
        private Button btnApply, btnCancel, btnLogout;

        private SettingCard sndOn, sndOff, sttOn, sttOff;

        // ---- check font support (giữ nguyên) ----
        [System.Runtime.InteropServices.DllImport("gdi32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        static extern int GetGlyphIndicesW(IntPtr hdc, string text, int count, ushort[] pgi, int flags);

        private bool FontSupportsText(string familyName, string sample)
        {
            try
            {
                using (var f = new Font(familyName, 10f))
                using (var bmp = new Bitmap(1, 1))
                using (var g = Graphics.FromImage(bmp))
                {
                    IntPtr hdc = g.GetHdc();
                    IntPtr hFont = f.ToHfont();
                    IntPtr old = NativeMethods.SelectObject(hdc, hFont);
                    ushort[] gi = new ushort[sample.Length];
                    int ret = GetGlyphIndicesW(hdc, sample, sample.Length, gi, 0);
                    NativeMethods.SelectObject(hdc, old);
                    NativeMethods.DeleteObject(hFont);
                    g.ReleaseHdc(hdc);
                    if (ret == 0) return false;
                    for (int i = 0; i < gi.Length; i++)
                        if (gi[i] == 0xFFFF) return false;
                    return true;
                }
            }
            catch { return false; }
        }
        private static class NativeMethods
        {
            [System.Runtime.InteropServices.DllImport("gdi32.dll")] public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
            [System.Runtime.InteropServices.DllImport("gdi32.dll")] public static extern bool DeleteObject(IntPtr hObject);
        }

        // ===== Constructor mới: nhận userId =====
        public SettingForm(Guid userId)
        {
            InitializeComponent();
            _currentUserId = userId;

            Font = new Font("Segoe UI", 10f);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            _fontFamilyPreview = this.Font.FontFamily.Name;

            Shown += (s, e) =>
            {
                BuildUI();
                BuildCards();
                LoadPreviewValues();     // CHỈ load UI, KHÔNG ghi DB
                UpdateFormColorsOnly();
                RedrawPillStates();
                ApplyLocalization();
            };
        }

        private void BuildUI()
        {
            BackColor = Color.White;
            ForeColor = Color.Black;

            header = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.White };
            btnBack = new BackButton { Left = 12, Top = 14 };
            btnBack.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            lblTitle = new Label
            {
                Text = "Cài đặt",
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI Semibold", 12f),
                Padding = new Padding(48, 0, 0, 0)
            };
            header.Controls.Add(btnBack);
            header.Controls.Add(lblTitle);
            Controls.Add(header);

            flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(16, 46, 16, 16),
                BackColor = Color.White,
                AutoScrollMargin = new Size(0, 8)
            };
            Controls.Add(flow);

            bottomBar = new Panel { Dock = DockStyle.Bottom, Height = 64, BackColor = Color.White, Padding = new Padding(16) };
            btnApply = new Button { Text = "Áp dụng", Width = 110, Height = 36, Anchor = AnchorStyles.Right | AnchorStyles.Bottom };
            btnCancel = new Button { Text = "Hủy", Width = 90, Height = 36, Anchor = AnchorStyles.Right | AnchorStyles.Bottom };
            btnLogout = new Button { Text = "Đăng xuất", Width = 110, Height = 36, Anchor = AnchorStyles.Left | AnchorStyles.Bottom };

            bottomBar.Controls.Add(btnApply);
            bottomBar.Controls.Add(btnCancel);
            bottomBar.Controls.Add(btnLogout);
            Controls.Add(bottomBar);
            bottomBar.BringToFront();

            bottomBar.Resize += (s, e) =>
            {
                btnLogout.Left = 16;
                btnLogout.Top = 14;

                btnCancel.Left = bottomBar.Width - btnCancel.Width - 16;
                btnApply.Left = btnCancel.Left - btnApply.Width - 8;
                btnCancel.Top = btnApply.Top = 14;
            };
            // đặt lần đầu
            btnLogout.Left = 16;
            btnLogout.Top = 14;
            btnCancel.Left = bottomBar.Width - btnCancel.Width - 16;
            btnApply.Left = btnCancel.Left - btnApply.Width - 8;
            btnCancel.Top = btnApply.Top = 14;

            // Áp dụng/Hủy chỉ đóng form, không lưu cấu hình nào cả (vì không dùng AppSettings)
            btnApply.Click += (s, e) => { DialogResult = DialogResult.OK; Close(); };
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            btnLogout.Click += (s, e) =>
            {
                if (MessageBox.Show("Bạn chắc chắn muốn đăng xuất?", "Đăng xuất",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                Application.Restart(); // quay về màn đăng nhập nếu app của bạn khởi động vào LoginForm
            };
        }

        private SettingCard MakeRowCard(Label leftLabel, Control rightControl, bool isFirst = false)
        {
            var card = new SettingCard
            {
                Width = 820,
                Height = 96,
                BaseColor = Color.FromArgb(248, 249, 250),
                BorderColor = Color.FromArgb(210, 213, 218),
                BorderThickness = 1,
                HoverLighten = 0.06f,
                Margin = isFirst ? new Padding(12, 28, 12, 8) : new Padding(12)
            };

            var row = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24, 12, 24, 12), BackColor = Color.Transparent };

            leftLabel.AutoSize = false;
            leftLabel.Width = 140;
            leftLabel.AutoEllipsis = true;
            leftLabel.TextAlign = ContentAlignment.MiddleLeft;
            leftLabel.BackColor = Color.Transparent;

            rightControl.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            row.Controls.Add(leftLabel);
            row.Controls.Add(rightControl);
            card.Controls.Add(row);

            void layout()
            {
                leftLabel.Top = (row.Height - leftLabel.Height) / 2;
                rightControl.Left = row.Width - rightControl.Width;
                rightControl.Top = (row.Height - rightControl.Height) / 2;
            }
            row.Resize += (s, e) => layout();
            layout();
            return card;
        }

        private void BuildCards()
        {
            // 1) ÂM THANH
            var pnlSound = BuildPills(out sndOn, out sndOff, 236, 42, out lSndOn, out lSndOff);
            lblSoundLeft = new Label { Text = "Âm thanh" };
            lSndOn.Text = "Bật tiếng"; lSndOff.Text = "Tắt tiếng";
            flow.Controls.Add(MakeRowCard(lblSoundLeft, pnlSound, isFirst: true));
            sndOn.Click += (s, e) => SetSound(true);
            sndOff.Click += (s, e) => SetSound(false);
            lSndOn.Click += (s, e) => SetSound(true);
            lSndOff.Click += (s, e) => SetSound(false);

            // 2) TRẠNG THÁI
            var pnlStatus = BuildPills(out sttOn, out sttOff, 236, 42, out lSttOn, out lSttOff);
            lblStatusLeft = new Label { Text = "Trạng thái" };
            lSttOn.Text = "Hoạt động"; lSttOff.Text = "Không hoạt động";
            flow.Controls.Add(MakeRowCard(lblStatusLeft, pnlStatus));
            sttOn.Click += (s, e) => SetStatus("Active");   // chỉ cập nhật DB khi click
            sttOff.Click += (s, e) => SetStatus("Inactive");
            lSttOn.Click += (s, e) => SetStatus("Active");
            lSttOff.Click += (s, e) => SetStatus("Inactive");

            // 3) PHÔNG CHỮ
            lblFontLeft = new Label { Text = "Phông chữ" };
            cmbFontFamily = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 220 };
            using (var ifc = new InstalledFontCollection())
            {
                var sample = "Tiếng Việt ă â ê ô ơ ư đ";
                var names = ifc.Families.Select(f => f.Name)
                                         .Where(n => FontSupportsText(n, sample))
                                         .OrderBy(n => n).ToArray();
                if (names.Length == 0) names = ifc.Families.Select(f => f.Name).OrderBy(n => n).ToArray();
                cmbFontFamily.Items.AddRange(names);
            }
            cmbFontFamily.SelectedIndexChanged += (s, e) =>
            {
                _fontFamilyPreview = (string)cmbFontFamily.SelectedItem;
                UIFontApplier.ApplyFont(this, _fontFamilyPreview, (int)this.Font.Size);
            };
            flow.Controls.Add(MakeRowCard(lblFontLeft, cmbFontFamily));
        }

        private Panel BuildPills(out SettingCard left, out SettingCard right, int pillWidth, int pillHeight, out Label leftLabel, out Label rightLabel)
        {
            var panel = new Panel { Width = pillWidth * 2 + 12, Height = pillHeight, BackColor = Color.Transparent };
            left = new SettingCard { Width = pillWidth, Height = pillHeight, BaseColor = Color.FromArgb(248, 249, 250), BorderColor = Color.FromArgb(210, 213, 218), BorderThickness = 1, HoverLighten = 0.06f, IsClickable = true, CornerRadius = pillHeight / 2 };
            right = new SettingCard { Width = pillWidth, Height = pillHeight, BaseColor = Color.FromArgb(248, 249, 250), BorderColor = Color.FromArgb(210, 213, 218), BorderThickness = 1, HoverLighten = 0.06f, IsClickable = true, CornerRadius = pillHeight / 2 };

            leftLabel = new Label { AutoSize = false, AutoEllipsis = true, Text = "Trái", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent };
            rightLabel = new Label { AutoSize = false, AutoEllipsis = true, Text = "Phải", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent };

            left.Controls.Add(leftLabel);
            right.Controls.Add(rightLabel);

            left.Left = 0; left.Top = 0;
            right.Left = pillWidth + 12; right.Top = 0;

            panel.Controls.Add(left);
            panel.Controls.Add(right);
            return panel;
        }

        private void UpdateFormColorsOnly()
        {
            var formBg = Color.White;
            var textCol = Color.Black;
            var cardBg = Color.FromArgb(248, 249, 250);
            var cardBorder = Color.FromArgb(210, 213, 218);

            BackColor = formBg;
            ForeColor = textCol;
            if (header != null) header.BackColor = formBg;
            if (lblTitle != null) lblTitle.ForeColor = textCol;
            if (btnBack != null) btnBack.ForeColor = textCol;
            if (flow != null) flow.BackColor = formBg;
            if (bottomBar != null) bottomBar.BackColor = Color.White;

            foreach (SettingCard card in flow.Controls.OfType<SettingCard>())
            {
                card.BaseColor = cardBg;
                card.BorderColor = cardBorder;
                card.HoverDarken = true;
                card.Invalidate();
            }
        }

        private void RedrawPillStates()
        {
            var accent = Color.FromArgb(0, 120, 215);
            var idle = Color.FromArgb(248, 249, 250);
            var idleText = Color.Black;

            if (sndOn != null && sndOff != null)
            {
                sndOn.BaseColor = _soundOnPreview ? accent : idle;
                sndOff.BaseColor = _soundOnPreview ? idle : accent;
                foreach (Label l in sndOn.Controls.OfType<Label>()) l.ForeColor = _soundOnPreview ? Color.White : idleText;
                foreach (Label l in sndOff.Controls.OfType<Label>()) l.ForeColor = _soundOnPreview ? idleText : Color.White;
                sndOn.Invalidate(); sndOff.Invalidate();
            }

            bool active = _statusPreview == "Active";
            if (sttOn != null && sttOff != null)
            {
                sttOn.BaseColor = active ? accent : idle;
                sttOff.BaseColor = active ? idle : accent;
                foreach (Label l in sttOn.Controls.OfType<Label>()) l.ForeColor = active ? Color.White : idleText;
                foreach (Label l in sttOff.Controls.OfType<Label>()) l.ForeColor = active ? idleText : Color.White;
                sttOn.Invalidate(); sttOff.Invalidate();
            }
        }

        private void SetSound(bool on)
        {
            _soundOnPreview = on;
            RedrawPillStates();
        }

        // ====== CẬP NHẬT SQL CHỈ KHI NGƯỜI DÙNG CLICK ======
        private void SetStatus(string s)
        {
            _statusPreview = (s == "Active") ? "Active" : "Inactive";
            RedrawPillStates();

            try
            {
                bool newIsActive = _statusPreview == "Active";
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("UPDATE dbo.Users SET IsActive = @isActive WHERE Id = @userId", conn))
                {
                    cmd.Parameters.AddWithValue("@isActive", newIsActive);
                    cmd.Parameters.AddWithValue("@userId", _currentUserId);
                    conn.Open();
                    int n = cmd.ExecuteNonQuery();
                    if (n == 0)
                    {
                        MessageBox.Show("Không tìm thấy người dùng để cập nhật.", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                MessageBox.Show("Trạng thái đã được cập nhật thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật trạng thái: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPreviewValues()
        {
            // KHÔNG gọi SetStatus ở đây để tránh ghi DB khi mở form.
            // Chỉ cập nhật UI theo giá trị mặc định hiện tại.
            RedrawPillStates();

            if (cmbFontFamily != null)
            {
                var want = _fontFamilyPreview ?? this.Font.FontFamily.Name;
                int idx = cmbFontFamily.FindStringExact(want);
                cmbFontFamily.SelectedIndex = idx >= 0 ? idx : cmbFontFamily.FindStringExact(this.Font.FontFamily.Name);
            }
        }

        private void ApplyLocalization()
        {
            // Chốt tiếng Việt cho UI
            lblTitle.Text = "Cài đặt";
            lblSoundLeft.Text = "Âm thanh";
            lblStatusLeft.Text = "Trạng thái";
            lblFontLeft.Text = "Phông chữ";

            lSndOn.Text = "Bật tiếng";
            lSndOff.Text = "Tắt tiếng";
            lSttOn.Text = "Hoạt động";
            lSttOff.Text = "Không hoạt động";

            btnApply.Text = "Áp dụng";
            btnCancel.Text = "Hủy";
            btnLogout.Text = "Đăng xuất";
        }
    }
}
