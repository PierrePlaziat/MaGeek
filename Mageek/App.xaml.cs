using MaGeek.Data;
using MaGeek.Utils;
using System.Windows;

namespace MaGeek
{


    public partial class App : Application
    {

        public static AppDbContext database = new AppDbContext();
        public static CardManager cardManager = new CardManager();
        public static AppState state = new AppState();

    }

}
