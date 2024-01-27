using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels.AppWindows;

namespace MageekFrontWpf.UI.Views.AppWindows
{

    public partial class CollectionEstimation : BaseWindow
    {

        public CollectionEstimation(CollectionEstimationViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }

    }

}
