using MageekFrontWpf.UI.ViewModels.AppWindows;
using PlaziatWpf.Mvvm;

namespace MageekFrontWpf.UI.Views.AppWindows
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
