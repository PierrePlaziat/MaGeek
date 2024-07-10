using MageekDesktop.UI.ViewModels.AppWindows;
using PlaziatWpf.Mvvm;

namespace MageekDesktop.UI.Views.AppWindows
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
