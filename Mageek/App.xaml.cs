using System.Windows;
using MaGeek.AppFramework;

namespace MaGeek
{

    /// <summary>
    /// Shouldnt be modified
    /// Only Exposes code architecture 
    /// </summary>
    public partial class App : Application
    {

        public static AppEvents Events  { get; } = new();
        public static AppConfig Config  { get; } = new();   
        public static AppState  State   { get; } = new();
        public static AppBiz    Biz     { get; } = new();

        public App() {}

    }

}
