using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace MaGeek
{

    public partial class App : Application
    {

        static MainWindow mainWindow;

        public static AppEvents Events { get; private set; } 
        public static AppConfig Config { get; private set; }
        public static AppState State { get; private set; }
        public static DeckImporter Importer { get; private set; }

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

        public static async Task<bool> IsUpdateAvailable() //TODO 
        {
            return false;
        }

        public static async Task UpdateSoftware()  //TODO 
        {
        }

        #endregion

    }

}
