using MaGeek.Data;
using System.IO;
using System.Windows;

namespace MaGeek
{

    public partial class App : Application
    {

        public static AppState State = new AppState();
        public static LocalDb Database = new LocalDb(); 
        public static CardManager CardManager = new CardManager();

        public App()
        {
            Directory.CreateDirectory(@"./CardsIllus");
        }
        
    }

}
