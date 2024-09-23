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
            this.Closing += PrintWindow_Closing;
        }

        private void PrintWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

    }

}