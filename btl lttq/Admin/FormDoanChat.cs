using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace btl_lttq.Admin
{
    public partial class FormDoanChat : Form
    {
        private readonly string connectionString =
            "Server=LAPTOP-LQAGRB3F;Database=MessengerDb;Trusted_Connection=True;TrustServerCertificate=True;";

        public FormDoanChat()
        {
            InitializeComponent();
        }

        private void FormDoanChat_Load(object sender, EventArgs e)
        {
            LoadGroups();

            // Gắn đúng menu cho đúng control
            lvGroups.ContextMenuStrip = contextGroups;
            lbMembers.ContextMenuStrip = contextMembers;

            // Chuột phải vào nhóm: chọn item trước & mở đúng menu nhóm
            lvGroups.MouseUp += (s, ev) =>
            {
                if (ev.Button != MouseButtons.Right) return;

                var hit = lvGroups.HitTest(ev.Location);
                if (hit.Item != null) hit.Item.Selected = true;

                // Bật/tắt nút Xóa nhóm khi có/không có chọn
                xóaNhómToolStripMenuItem.Enabled = lvGroups.SelectedItems.Count > 0;
                contextGroups.Show(lvGroups, ev.Location);
            };

            // Chuột phải vào thành viên: chọn item trước & mở đúng menu thành viên
            lbMembers.MouseUp += (s, ev) =>
            {
                if (ev.Button != MouseButtons.Right) return;

                int idx = lbMembers.IndexFromPoint(ev.Location);
                if (idx != ListBox.NoMatches) lbMembers.SelectedIndex = idx;

                // Bật/tắt nút Xóa/Thêm thành viên
                xóaThànhViênToolStripMenuItem.Enabled = lbMembers.SelectedItem != null;
                thêmThànhViênToolStripMenuItem.Enabled = lvGroups.SelectedItems.Count > 0;

                contextMembers.Show(lbMembers, ev.Location);
            };
        }

        // ======= LOAD =======
        private void LoadGroups()
        {
            lvGroups.Items.Clear();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"SELECT Id, Title 
                      FROM dbo.Conversations 
                      WHERE IsGroup = 1 
                      ORDER BY CreatedAt DESC", conn);

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        var it = new ListViewItem(r["Title"].ToString())
                        {
                            Tag = r["Id"].ToString()
                        };
                        lvGroups.Items.Add(it);
                    }
                }
            }

            lvGroups.View = View.List;
            lvGroups.FullRowSelect = true;
            lvGroups.MultiSelect = false;
        }

        private void lvGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbMembers.Items.Clear();
            if (lvGroups.SelectedItems.Count == 0) return;

            string groupId = lvGroups.SelectedItems[0].Tag.ToString();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"SELECT u.DisplayName
                      FROM dbo.ConversationMembers m
                      JOIN dbo.Users u ON u.Id = m.UserId
                      WHERE m.ConversationId = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", groupId);

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read()) lbMembers.Items.Add(r["DisplayName"].ToString());
                }
            }
        }

        // ======= MENU NHÓM =======
        private void thêmNhómToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string tenNhom = Interaction.InputBox("Nhập tên nhóm mới:", "Thêm nhóm");
            if (string.IsNullOrWhiteSpace(tenNhom)) return;

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"INSERT INTO dbo.Conversations (Id, Title, IsGroup, CreatedBy, CreatedAt)
                      VALUES (NEWID(), @Title, 1, (SELECT TOP 1 Id FROM dbo.Users), SYSDATETIME())", conn);
                cmd.Parameters.AddWithValue("@Title", tenNhom);
                cmd.ExecuteNonQuery();
            }

            LoadGroups();
            MessageBox.Show("✅ Đã thêm nhóm mới!");
        }

        private void xóaNhómToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lvGroups.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn nhóm cần xóa!");
                return;
            }

            var id = lvGroups.SelectedItems[0].Tag.ToString();
            if (MessageBox.Show("Bạn có chắc muốn xóa nhóm này?", "Xác nhận",
                                MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM dbo.Conversations WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }

            lbMembers.Items.Clear();
            LoadGroups();
            MessageBox.Show("🗑️ Đã xóa nhóm!");
        }

        // ======= MENU THÀNH VIÊN =======
        private void thêmThànhViênToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lvGroups.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn nhóm trước!");
                return;
            }

            string groupId = lvGroups.SelectedItems[0].Tag.ToString();
            string email = Interaction.InputBox("Nhập Email của thành viên cần thêm:", "Thêm thành viên");

            if (string.IsNullOrWhiteSpace(email)) return;

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"INSERT INTO dbo.ConversationMembers (ConversationId, UserId)
              SELECT @GroupId, u.Id
              FROM dbo.Users u
              WHERE u.Email = @Email
                AND NOT EXISTS (
                    SELECT 1 FROM dbo.ConversationMembers 
                    WHERE ConversationId = @GroupId AND UserId = u.Id
                )", conn);

                cmd.Parameters.AddWithValue("@GroupId", groupId);
                cmd.Parameters.AddWithValue("@Email", email);

                int rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                    MessageBox.Show("✅ Đã thêm thành viên vào nhóm!");
                else
                    MessageBox.Show("⚠️ Email không tồn tại hoặc thành viên đã có trong nhóm!");
            }

            // Refresh danh sách thành viên
            lvGroups_SelectedIndexChanged(null, null);
        }


        private void xóaThànhViênToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lvGroups.SelectedItems.Count == 0 || lbMembers.SelectedItem == null)
            {
                MessageBox.Show("Chọn nhóm và thành viên cần xóa!");
                return;
            }

            string groupId = lvGroups.SelectedItems[0].Tag.ToString();
            string displayName = lbMembers.SelectedItem.ToString();

            if (MessageBox.Show($"Xóa '{displayName}' khỏi nhóm này?", "Xác nhận",
                                MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"DELETE FROM dbo.ConversationMembers
              WHERE ConversationId = @GroupId
                AND UserId = (SELECT TOP 1 Id FROM dbo.Users WHERE DisplayName = @DisplayName)", conn);
                cmd.Parameters.AddWithValue("@GroupId", groupId);
                cmd.Parameters.AddWithValue("@DisplayName", displayName);
                cmd.ExecuteNonQuery();
            }

            lvGroups_SelectedIndexChanged(null, null);
            MessageBox.Show($"🗑️ Đã xóa '{displayName}' khỏi nhóm!");
        }
    }
}
