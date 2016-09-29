using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace PreventSleep
{
    public partial class PreventSleep : Form
    {
        [FlagsAttribute]
        public enum EXECUTION_STATE : uint
        {
            NULL = 0,
            ES_SYSTEM_REQUIRED = 0x00000001,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_CONTINUOUS = 0x80000000,
        }

        [DllImport("kernel32.dll")]
        extern static EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        public PreventSleep()
        {
            InitializeComponent();

            var timer = new Timer();
            timer.Tick += new EventHandler(this.OnTimer);
            timer.Interval = 10 * 1000;
            timer.Start();

            notifyIcon1.Text = "PreventSleep";

            // prevent system sleep
            SetThreadExecutionState(EXECUTION_STATE.ES_SYSTEM_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS);
        }

        private void OnTimer(object sender, EventArgs e)
        {
            // prevent display from turning off
            SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED);
        }

        //
        // see also... http://dobon.net/vb/dotnet/form/hideformwithtrayicon.html
        //
        protected override CreateParams CreateParams
        {
            [System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.SecurityAction.LinkDemand,
                Flags = System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                const int WS_EX_TOOLWINDOW = 0x80;
                const long WS_POPUP = 0x80000000L;
                const int WS_VISIBLE = 0x10000000;
                const int WS_SYSMENU = 0x80000;
                const int WS_MAXIMIZEBOX = 0x10000;

                CreateParams cp = base.CreateParams;
                cp.ExStyle = WS_EX_TOOLWINDOW;
                cp.Style = unchecked((int)WS_POPUP) |
                    WS_VISIBLE | WS_SYSMENU | WS_MAXIMIZEBOX;
                cp.Width = 0;
                cp.Height = 0;

                return cp;
            }
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;

            // clear ES_SYSTEM_REQUIRED
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);

            Application.Exit();
        }
    }
}
