using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Forms;

namespace btl_lttq.Friendprofile
{
    public partial class AddFriendForm : Form
    {
        private List<FriendRequest> allRequests = new List<FriendRequest>();
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["MessengerDb"].ConnectionString;

        private readonly FriendListForm _friendListForm;

        // 👇 user thật sự đang đăng nhập
        private readonly Guid _currentUserId;
        private readonly string _currentUsername;

        // ctor CHÍNH: luôn truyền user hiện tại
        public AddFriendForm(FriendListForm friendListForm, Guid currentUserId, string currentUsername)
        {
            InitializeComponent();
            _friendListForm = friendListForm;
            _currentUserId = currentUserId;
            _currentUsername = currentUsername;
        }

        // ctor fallback nếu ai đó gọi cũ
        public AddFriendForm() : this(null, Guid.Empty, null) { }

        private void AddFriendForm_Load(object sender, EventArgs e)
        {
            flowRequests.AutoScroll = true;
            flowRequests.WrapContents = false;
            flowRequests.FlowDirection = FlowDirection.TopDown;

            // nếu vẫn chưa có id thì fallback anninh (chỉ dành cho test)
            Guid useId = _currentUserId;
            string useName = _currentUsername;

            if (useId == Guid.Empty)
            {
                useName = string.IsNullOrEmpty(useName) ? "anninh" : useName;
                useId = GetUserId(useName);
            }

            LoadFriendRequests(useId);

            // placeholder
            txtSearch.ForeColor = Color.Gray;
            txtSearch.Text = "Tìm lời kết bạn hoặc người dùng";
            txtSearch.Font = new Font("Segoe UI", 12, FontStyle.Italic);
            txtSearch.GotFocus += RemovePlaceholder;
            txtSearch.LostFocus += AddPlaceholder;
            txtSearch.LostFocus += (s, e2) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text) ||
                    txtSearch.Text == "Tìm lời kết bạn hoặc người dùng")
                    DisplayFriendRequests(allRequests);
            };

            this.ActiveControl = null;
        }

        // 🔹 load cả lời mời đến + người có thể gửi lời mời
        private void LoadFriendRequests(Guid currentUserId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    var all = new List<FriendRequest>();

                    // 1. lời mời đến
                    string sqlInvite = @"
                        SELECT 
                            f.Id AS FriendshipId, 
                            u.DisplayName, 
                            u.AvatarUrl,
                            u.Id AS SenderId,
                            u.UserName
                        FROM Friendships f
                        JOIN Users u ON u.Id = f.RequesterId
                        WHERE f.AddresseeId = @userId AND f.Status = 0";

                    using (SqlCommand cmd = new SqlCommand(sqlInvite, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", currentUserId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                all.Add(new FriendRequest
                                {
                                    FriendshipId = reader.GetGuid(0),
                                    DisplayName = reader.GetString(1),
                                    AvatarUrl = reader.IsDBNull(2) ? null : reader.GetString(2),
                                    SenderId = reader.GetGuid(3),
                                    FriendUsername = reader.GetString(4),
                                    RequestType = "INVITE"
                                });
                            }
                        }
                    }

                    // 2. người chưa kết bạn + chưa pending 2 chiều + không phải chính mình
                    string sqlNew = @"
                        SELECT u.Id, u.DisplayName, u.AvatarUrl, u.UserName
                        FROM Users u
                        WHERE 
                            u.Id IS NOT NULL
                            AND u.Id <> @me
                            AND u.Id NOT IN (
                                SELECT 
                                    CASE 
                                        WHEN f.RequesterId = @me THEN f.AddresseeId
                                        WHEN f.AddresseeId = @me THEN f.RequesterId
                                    END
                                FROM Friendships f
                                WHERE (f.RequesterId = @me OR f.AddresseeId = @me)
                                      AND f.Status IN (0,1)
                            )
                        ORDER BY u.DisplayName;";

                    using (SqlCommand cmd2 = new SqlCommand(sqlNew, conn))
                    {
                        cmd2.Parameters.AddWithValue("@me", currentUserId);
                        using (SqlDataReader reader = cmd2.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                all.Add(new FriendRequest
                                {
                                    FriendshipId = Guid.Empty,
                                    DisplayName = reader.GetString(1),
                                    AvatarUrl = reader.IsDBNull(2) ? null : reader.GetString(2),
                                    SenderId = reader.GetGuid(0),
                                    FriendUsername = reader.GetString(3),
                                    RequestType = "SEND"
                                });
                            }
                        }
                    }

                    allRequests = all
                        .OrderByDescending(r => r.RequestType == "INVITE")
                        .ThenBy(r => r.DisplayName)
                        .ToList();

                    DisplayFriendRequests(allRequests);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi tải danh sách: " + ex.Message);
            }
        }

        private void DisplayFriendRequests(List<FriendRequest> requests)
        {
            flowRequests.Controls.Clear();
            flowRequests.Padding = new Padding(10);

            foreach (var req in requests)
            {
                Panel p = new Panel
                {
                    Width = flowRequests.Width - 35,
                    Height = 70,
                    Margin = new Padding(0, 0, 0, 10),
                    BackColor = Color.WhiteSmoke
                };

                // avatar
                PictureBox avatar = new PictureBox
                {
                    Size = new Size(50, 50),
                    Location = new Point(10, 10),
                    SizeMode = PictureBoxSizeMode.Zoom
                };
                string path = Path.Combine(Application.StartupPath, "Images", req.AvatarUrl ?? "");
                if (File.Exists(path))
                    avatar.Image = Image.FromFile(path);
                else
                    avatar.BackColor = Color.LightGray;
                avatar.Paint += (s, e) =>
                {
                    var gp = new System.Drawing.Drawing2D.GraphicsPath();
                    gp.AddEllipse(0, 0, avatar.Width - 1, avatar.Height - 1);
                    avatar.Region = new Region(gp);
                };

                Label lblName = new Label
                {
                    Text = req.DisplayName,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    AutoSize = true,
                    Location = new Point(70, 20)
                };

                // nút thông tin (giữ lại)
                Button btnInfo = new Button
                {
                    Text = "Thông tin",
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.White,
                    BackColor = Color.MediumSeaGreen,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(90, 30),
                    Location = new Point(p.Width - 220, 20),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };
                btnInfo.FlatAppearance.BorderSize = 0;
                btnInfo.Click += (s, e) =>
                {
                    var profileForm = new ProfileFriendForm(req.SenderId);
                    profileForm.StartPosition = FormStartPosition.CenterScreen;
                    profileForm.ShowDialog();
                };

                if (req.RequestType == "INVITE")
                {
                    // lời mời đến
                    Button btnAccept = new Button
                    {
                        Text = "Chấp nhận",
                        Font = new Font("Segoe UI", 9),
                        ForeColor = Color.White,
                        BackColor = Color.RoyalBlue,
                        FlatStyle = FlatStyle.Flat,
                        Size = new Size(90, 30),
                        Location = new Point(p.Width - 320, 20),
                        Anchor = AnchorStyles.Top | AnchorStyles.Right
                    };
                    btnAccept.FlatAppearance.BorderSize = 0;
                    btnAccept.Click += (s, e) => AcceptRequest(req.FriendshipId);

                    Button btnDelete = new Button
                    {
                        Text = "Xóa",
                        Font = new Font("Segoe UI", 9),
                        ForeColor = Color.White,
                        BackColor = Color.LightCoral,
                        FlatStyle = FlatStyle.Flat,
                        Size = new Size(80, 30),
                        Location = new Point(p.Width - 120, 20),
                        Anchor = AnchorStyles.Top | AnchorStyles.Right
                    };
                    btnDelete.FlatAppearance.BorderSize = 0;
                    btnDelete.Click += (s, e) => DeleteRequest(req.FriendshipId);

                    p.Controls.Add(btnAccept);
                    p.Controls.Add(btnInfo);
                    p.Controls.Add(btnDelete);
                }
                else
                {
                    // người có thể gửi lời mời
                    Button btnSend = new Button
                    {
                        Text = "+ Gửi lời mời",
                        Font = new Font("Segoe UI", 9),
                        ForeColor = Color.White,
                        BackColor = Color.RoyalBlue,
                        FlatStyle = FlatStyle.Flat,
                        Size = new Size(110, 30),
                        Location = new Point(p.Width - 120, 20),
                        Anchor = AnchorStyles.Top | AnchorStyles.Right
                    };
                    btnSend.FlatAppearance.BorderSize = 0;
                    btnSend.Click += (s, e) => SendFriendRequest(req.SenderId);

                    p.Controls.Add(btnInfo);
                    p.Controls.Add(btnSend);
                }

                p.Controls.Add(avatar);
                p.Controls.Add(lblName);
                flowRequests.Controls.Add(p);
            }
        }

        private void SendFriendRequest(Guid receiverId)
        {
            try
            {
                Guid currentUserId = _currentUserId != Guid.Empty ? _currentUserId : GetUserId(_currentUsername ?? "anninh");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"
                        INSERT INTO Friendships (Id, RequesterId, AddresseeId, Status, CreatedAt)
                        VALUES (NEWID(), @from, @to, 0, SYSDATETIME())";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@from", currentUserId);
                    cmd.Parameters.AddWithValue("@to", receiverId);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("✅ Đã gửi lời mời kết bạn!");
                LoadFriendRequests(currentUserId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi khi gửi lời mời: " + ex.Message);
            }
        }

        private void AcceptRequest(Guid friendshipId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE Friendships SET Status=1, UpdatedAt=SYSDATETIME() WHERE Id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", friendshipId);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("✅ Đã chấp nhận!");
                var uid = _currentUserId != Guid.Empty ? _currentUserId : GetUserId(_currentUsername ?? "anninh");
                LoadFriendRequests(uid);
                _friendListForm?.ReloadFriends();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi khi chấp nhận: " + ex.Message);
            }
        }

        private void DeleteRequest(Guid friendshipId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM Friendships WHERE Id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", friendshipId);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("🗑️ Đã xóa lời mời!");
                var uid = _currentUserId != Guid.Empty ? _currentUserId : GetUserId(_currentUsername ?? "anninh");
                LoadFriendRequests(uid);
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi khi xóa: " + ex.Message);
            }
        }

        private Guid GetUserId(string username)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Id FROM Users WHERE UserName=@u", conn);
                cmd.Parameters.AddWithValue("@u", username);
                return (Guid)cmd.ExecuteScalar();
            }
        }

        private void btnFriend_Click(object sender, EventArgs e)
        {
            this.Hide();
            _friendListForm?.Show();
            _friendListForm?.ReloadFriends();
            this.Close();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(keyword) || txtSearch.Text == "Tìm lời kết bạn hoặc người dùng")
            {
                DisplayFriendRequests(allRequests);
                return;
            }

            string keywordNoDiacritics = RemoveDiacritics(keyword);
            var filtered = allRequests.Where(r =>
            {
                string name = r.DisplayName?.ToLower() ?? "";
                string nameNoDiacritics = RemoveDiacritics(name);
                return name.Contains(keyword) || nameNoDiacritics.Contains(keywordNoDiacritics);
            }).ToList();

            DisplayFriendRequests(filtered);

            if (filtered.Count == 0)
                MessageBox.Show("Không tìm thấy người phù hợp.", "Thông báo");
        }

        private static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            string normalized = text.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            foreach (char c in normalized)
            {
                var cat = CharUnicodeInfo.GetUnicodeCategory(c);
                if (cat != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        private void RemovePlaceholder(object sender, EventArgs e) { /* ... */ }
        private void AddPlaceholder(object sender, EventArgs e) { /* ... */ }
    }

    public class FriendRequest
    {
        public Guid FriendshipId { get; set; }
        public Guid SenderId { get; set; }
        public string DisplayName { get; set; }
        public string AvatarUrl { get; set; }
        public string FriendUsername { get; set; }
        public string RequestType { get; set; }
    }
}
