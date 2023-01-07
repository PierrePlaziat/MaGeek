using System;
using System.IO;
using System.Windows;

namespace MaGeek
{

    public partial class App : Application
    {

        public static DbManager DB = new DbManager();
        public static CardManager CARDS = new CardManager();
        public static AppState STATE = new AppState();

        public App()
        {
            InitFolders();
            DB.InitDb();
        }

        internal static void Restart()
        {
            System.Diagnostics.Process.Start(ResourceAssembly.Location);
            Current.Shutdown();
        }

        #region Folders

        public static string RoamingFolder { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MaGeek"); } }
        public static string ImageFolder { get { return Path.Combine(App.RoamingFolder, "CardsIllus"); } }

        private void InitFolders()
        {
            if (!File.Exists(RoamingFolder)) Directory.CreateDirectory(RoamingFolder);
            if (!File.Exists(ImageFolder)) Directory.CreateDirectory(ImageFolder);
        }

        #endregion

    }

}
