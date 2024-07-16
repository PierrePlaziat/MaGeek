using System.Windows;
using System.Windows.Forms;

namespace MageekDesktopServer
{

    public partial class App : System.Windows.Application
    {
        public static NotifyIcon icon;
        private static Queue<string> output = new Queue<string>();
        static public ConsoleAppManager manager { get; set; }
        static MageekDesktopServer.MainWindow logWin;

        protected override void OnStartup(StartupEventArgs e)
        {

            ConfigureTrayIcon();
            base.OnStartup(e);
            manager = new ConsoleAppManager("D:\\PROJECTS\\VS\\MaGeek\\Core\\MageekServer\\bin\\Debug\\net8.0\\MageekServer.exe");
            logWin = new MainWindow(manager);
            manager.ExecuteAsync();
        }

        private void ConfigureTrayIcon()
        {
            icon = new NotifyIcon();
            icon.MouseClick += Icon_MouseClick;
            icon.Icon = new System.Drawing.Icon("Resources\\icon.ico");
            icon.Visible = true;
            icon.ContextMenuStrip = new();
            icon.ContextMenuStrip.BackColor = System.Drawing.Color.Black;
            icon.ContextMenuStrip.ForeColor = System.Drawing.Color.White;
            icon.ContextMenuStrip.ShowImageMargin = false;
            icon.ContextMenuStrip.Items.Add("Logs", new System.Drawing.Bitmap("Resources\\icon.ico"), ContextMenu_Logs);
            icon.ContextMenuStrip.Items.Add("Quit", new System.Drawing.Bitmap("Resources\\icon.ico"), ContextMenu_Quit);
            icon.Text = "Mageek Desktop Server";
        }

        private void ContextMenu_Logs(object? sender, EventArgs e)
        {
            logWin.Show();
        }

        private void ContextMenu_Quit(object? sender, EventArgs e)
        {
            manager.EndProcess();
            Current.Shutdown();
        }

        private static void Icon_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                logWin.Show();
            }
            else if (e.Button == MouseButtons.Right)
            {
                ((NotifyIcon)sender).ContextMenuStrip?.Show();
            }
        }

    }

}
