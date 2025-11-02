using btl_lttq.ChatClient;
using btl_lttq.FacebookLite;
using btl_lttq.Friendprofile;
using btl_lttq.Login;
using btl_lttq.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace btl_lttq
{
    internal static class Program
    {
       [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMenu());
        }
    }
}
