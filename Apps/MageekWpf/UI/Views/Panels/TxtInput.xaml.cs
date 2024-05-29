using MageekFrontWpf.UI.ViewModels.AppWindows;
using PlaziatWpf.Mvvm;

namespace MageekFrontWpf.UI.Views.AppWindows
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
