using System.Windows;
using MaGeek.AppBusiness;
using MaGeek.AppData;
using MaGeek.AppFramework;
using Plaziat.CommonWpf;

namespace MaGeek
{

    public partial class App : Application
    {

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
            MageekTranslator.LoadTranslation();
        }

        public static void Restart()
        {
            MessageBoxHelper.ShowMsg("App will restart now.");
            System.Diagnostics.Process.Start(App.ResourceAssembly.Location);
            App.Current.Shutdown();
        }


    }

}
