using System;
using System.IO;
using System.Windows;
using MaGeek.Data;
using MaGeek.Services;

namespace MaGeek
{

    public partial class App : Application
    {

        public static LocalDb Database = new LocalDb(); 
        public static MageekManager MaGeek = new MageekManager();
        public static AppState State = new AppState();
        public static LanguageManager Lang = new LanguageManager();

        public static string RoamingFolder { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MaGeek"); } }
        public static string ImageFolder { get { return Path.Combine(App.RoamingFolder, "CardsIllus"); } }

        public App()
        {
            if (!File.Exists(RoamingFolder)) Directory.CreateDirectory(RoamingFolder);
            if (!File.Exists(ImageFolder)) Directory.CreateDirectory(ImageFolder);
            Database.InitDb();
        }

        internal static void Restart()
        {
            System.Diagnostics.Process.Start(ResourceAssembly.Location);
            Current.Shutdown();
        }

    }

}
