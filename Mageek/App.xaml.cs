using Plaziat.CommonWpf;
using System;
using System.IO;
using System.Windows;

namespace MaGeek
{

    public partial class App : Application
    {

        public static DbManager DB;
        public static CardManager CARDS = new CardManager();
        public static AppState STATE = new AppState();
        public static LanguageManager LANG = new LanguageManager();

        public static string RoamingFolder { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MaGeek"); } }
        public static string ImageFolder { get { return Path.Combine(App.RoamingFolder, "CardsIllus"); } }

        public App()
        {
            InitDb();
            if (!File.Exists(RoamingFolder)) Directory.CreateDirectory(RoamingFolder);
            if (!File.Exists(ImageFolder)) Directory.CreateDirectory(ImageFolder);
            DB.InitDb();
        }

        private void InitDb()
        {
            if (File.Exists(App.RoamingFolder + "\\DbToRestore.db"))
            {
                File.Delete(Path.Combine(App.RoamingFolder, "MaGeek.db"));
                File.Copy(App.RoamingFolder + "\\DbToRestore.db", Path.Combine(App.RoamingFolder, "MaGeek.db"));
                File.Delete(App.RoamingFolder + "\\DbToRestore.db");
            }
            DB = new DbManager(); 
        }

        internal static void Restart()
        {
            System.Diagnostics.Process.Start(ResourceAssembly.Location);
            Current.Shutdown();
        }

    }

}
