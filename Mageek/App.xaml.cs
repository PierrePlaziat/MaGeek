using System.Windows;
using MaGeek.AppFramework;
using Plaziat.CommonWpf;

namespace MaGeek
{

    /// <summary>
    /// Shouldnt be modified
    /// Only exposes AppFramework
    /// </summary>
    public partial class App : Application
    {

        public static AppEvents Events { get; private set; } 
        public static AppConfig Config { get; private set; }
        public static AppState State { get; private set; }
        public static AppBiz Biz { get; private set; }

        public App() {
            Events = new();
            Config = new();
            State = new();
            Biz = new();
        }

        public static void Restart()
        {
            MessageBoxHelper.ShowMsg("App will restart now.");
            System.Diagnostics.Process.Start(App.ResourceAssembly.Location);
            App.Current.Shutdown();
        }


    }

}
