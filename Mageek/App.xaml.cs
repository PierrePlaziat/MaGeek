using System.Windows;
using MaGeek.Data;

namespace MaGeek
{

    public partial class App : Application
    {

        public static LocalDb Database = new LocalDb(); 
        public static CardManager CardManager = new CardManager();
        public static AppState State = new AppState();

        public App()
        {
            Database.InitDb();
        }

        internal static void Restart()
        {
            System.Diagnostics.Process.Start(ResourceAssembly.Location);
            Current.Shutdown();
        }

    }

}
