﻿using System.Diagnostics;
using System.Windows;

namespace MaGeek
{

    public partial class App : Application
    {

        static MainWindow mainWindow;

        public static AppEvents Events { get; private set; } 
        public static AppConfig Config { get; private set; }
        public static AppState State { get; private set; }
        public static MageekDeckImporter Importer { get; private set; }

        public App() {
            Events = new();
            Config = new();
            State = new();
            Importer = new();
        }

        #region Methods

        internal static void LaunchMainWin()
        {
            mainWindow = new MainWindow();
            mainWindow.Show();
        }

        internal static void HyperLink(string v)
        {
            Process.Start(new ProcessStartInfo(v) { UseShellExecute = true });
        }

        public static void Restart()
        {
            Process.Start(Process.GetCurrentProcess().MainModule.FileName);
            Current.Shutdown();
        }

        internal static bool IsUpdateAvailable()
        {
            //TODO UpdateSoftware
            return false;
        }

        internal static void UpdateSoftware()
        {
            //TODO UpdateSoftware
        }

        #endregion

    }

}
