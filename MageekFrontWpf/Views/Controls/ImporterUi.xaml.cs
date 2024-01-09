using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.ViewModels;

namespace MaGeek.UI.Menus
{

    public partial class ImporterUi : BaseUserControl
    {
        public ImporterUi(ImporterUiViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }

    }
}
