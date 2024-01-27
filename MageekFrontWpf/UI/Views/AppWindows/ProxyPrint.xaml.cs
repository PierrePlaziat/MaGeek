using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels.AppWindows;
using System.ComponentModel;

namespace MageekFrontWpf.UI.Views.AppWindows
{

    public partial class ProxyPrint : BaseWindow, INotifyPropertyChanged
    {

        public ProxyPrint(ProxyPrintViewModel vm)
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