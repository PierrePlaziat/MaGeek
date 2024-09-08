using MageekDesktopClient.UI.ViewModels.AppWindows;
using PlaziatWpf.Mvvm;

namespace MageekDesktopClient.UI.Views.AppWindows
{

    public partial class TxtInput : BaseUserControl
    {

        public TxtInput(TxtInputViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }

    }

}
