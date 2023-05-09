using System;
using System.Diagnostics;
using System.Windows;
using MaGeek.AppBusiness;
using MaGeek.AppData;
using MaGeek.AppFramework;
using Plaziat.CommonWpf;

namespace MaGeek
{

    public partial class App : Application
    {

        static MainWindow mainWin;

        public static AppEvents Events { get; private set; } 
        public static AppConfig Config { get; private set; }
        public static AppState State { get; private set; }
        public static SqliteDbManager DB { get; private set; }
        public static MageekImporter Importer { get; private set; }

        public App() {
            Events = new();
            Config = new();
            State = new();
            DB = new();
            Importer = new();
        }

        public static void Restart()
        {
            AppLogger.ShowMsg("App should restart now.");
            Process.Start(ResourceAssembly.Location);
            Current.Shutdown();
        }

        internal static void HyperLink(string v)
        {
            Process.Start(new ProcessStartInfo(v) { UseShellExecute = true });
        }

        internal static void LaunchMainWin()
        {
            mainWin = new MainWindow();
            mainWin.Show();
        }
    }

}
