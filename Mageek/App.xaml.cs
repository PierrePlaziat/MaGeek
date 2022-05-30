using MaGeek.Data;
using System.Windows;

namespace MaGeek
{

    public partial class App : Application
    {

        public static SqliteContext database = new SqliteContext();
        public static CardManager cardManager = new CardManager();
        public static AppState state = new AppState();

    }

}
