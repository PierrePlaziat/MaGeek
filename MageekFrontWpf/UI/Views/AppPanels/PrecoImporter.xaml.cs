using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels.AppWindows;

namespace MageekFrontWpf.UI.Views.AppWindows
{

    public partial class PrecoList : BaseUserControl
    {

        public PrecoList(PrecoListViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }

    }

}
