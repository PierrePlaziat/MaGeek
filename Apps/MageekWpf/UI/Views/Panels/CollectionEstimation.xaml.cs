using MageekDesktop.UI.ViewModels.AppWindows;
using PlaziatWpf.Mvvm;

namespace MageekDesktop.UI.Views.AppWindows
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
