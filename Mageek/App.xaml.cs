using MaGeek.Data;
using System.IO;
using System.Windows;

namespace MaGeek
{

    public partial class App : Application
    {

        public static AppState state = new AppState();
        public static SqliteContext database = new SqliteContext();
        public static CardManager cardManager = new CardManager();

        public App()
        {
            Directory.CreateDirectory(@"./CardsIllus");
        }
        
    }

}
