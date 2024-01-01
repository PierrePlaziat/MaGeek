using System.Diagnostics;
using System.Windows;
using MageekFrontWpf;
using MageekService.Data.Collection;

namespace MaGeek
{

    public partial class App : Application
    {

        static MainWindow mainWindow;

        public static AppEvents Events { get; private set; } 
        public static AppState State { get; private set; }

        public static ListImporter Importer { get; private set; }
        public static UserSettings Config { get; private set; }

        public App() {
            Events = new();
            Config = new();
            State = new();
            Importer = new();
        }

        internal static void LaunchMainWin()
        {
            mainWindow = new MainWindow();
            mainWindow.Show();
        }

        public static void Restart()
        {
            Process.Start(Process.GetCurrentProcess().MainModule.FileName);
            Current.Shutdown();
        }

    }

}
