using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.ViewModels;

namespace MaGeek.UI.Menus
{

    public partial class BottomBar : BaseUserControl
    {

        public BottomBar(StateBarViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }

    }

}
