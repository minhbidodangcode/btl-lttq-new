using System;
using System.Windows.Forms;

namespace btl_lttq.Admin
{
    public partial class FormMenu : Form
    {
        public FormMenu()
        {
            InitializeComponent();

           
            this.IsMdiContainer = true;
        }

        private void tàiKhoảnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Ẩn/đóng các form con đang mở
            foreach (Form frmChild in this.MdiChildren)
            {
                frmChild.Close(); // hoặc frmChild.Hide() nếu bạn muốn chỉ ẩn không đóng
            }

            FormTaiKhoan frm = new FormTaiKhoan();
            frm.MdiParent = this;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

        private void đoạnChatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Ẩn/đóng các form con đang mở
            foreach (Form frmChild in this.MdiChildren)
            {
                frmChild.Close(); // hoặc frmChild.Hide()
            }

            FormDoanChat frm = new FormDoanChat();
            frm.MdiParent = this;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

    }
}
