using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels.AppWindows;

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
