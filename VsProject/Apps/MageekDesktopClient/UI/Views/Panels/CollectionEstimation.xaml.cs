using MageekDesktopClient.UI.ViewModels.AppWindows;
using PlaziatWpf.Mvvm;

namespace MageekDesktopClient.UI.Views.AppWindows
{

    public partial class CollecEstimation : BaseUserControl
    {

        public CollecEstimation(CollecEstimationViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }

    }

}
