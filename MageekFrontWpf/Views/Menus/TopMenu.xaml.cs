using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.ViewModels;

namespace MaGeek.UI.Menus
{

    public partial class TopMenu : BaseUserControl
    {

        public TopMenu(TopMenuViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }

    }

}
