using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.ViewModels;
using System.Windows;

namespace MaGeek
{

    public partial class MainWindow : BaseWindow
    {

        public MainWindow(MainWindowViewModel vm)
        {
            Application.Current.MainWindow = this;
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
            DataContext = vm;
            InitializeComponent();
        }

    }

}
