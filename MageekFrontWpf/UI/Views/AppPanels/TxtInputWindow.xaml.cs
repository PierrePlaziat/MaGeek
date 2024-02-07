using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels.AppWindows;

namespace MageekFrontWpf.UI.Views.AppWindows
{

    public partial class TxtInputWindow : BaseUserControl
    {

        public TxtInputWindow(TxtInputViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }

    }

}
