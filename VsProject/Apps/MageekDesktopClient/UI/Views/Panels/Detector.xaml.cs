using MageekDesktopClient.UI.ViewModels.AppPanels;
using PlaziatWpf.Mvvm;

namespace MageekDesktopClient.UI.Views.AppPanels
{

    public partial class Detector : BaseUserControl
    {

        public Detector(DetectorViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
            vm.Init(VideoViewInstance, MyCanvas);
        }

    }
}
