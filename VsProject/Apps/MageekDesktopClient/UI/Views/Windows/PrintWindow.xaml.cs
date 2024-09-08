using MageekDesktopClient.UI.ViewModels.AppWindows;
using PlaziatWpf.Mvvm;
using System.ComponentModel;

namespace MageekDesktopClient.UI.Views.AppWindows
{

    public partial class PrintWindow : BaseWindow, INotifyPropertyChanged
    {

        public PrintWindow(PrintWindowViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }

        private void CheckBox_IncludeBasicLands(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void CheckBox_OnlyMissing(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }

}