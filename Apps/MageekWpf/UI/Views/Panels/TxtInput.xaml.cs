using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels.AppWindows;

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
