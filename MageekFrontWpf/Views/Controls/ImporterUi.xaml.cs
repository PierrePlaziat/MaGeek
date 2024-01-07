using MageekFrontWpf.ViewModels;

namespace MaGeek.UI.Menus
{

    public partial class ImporterUi : TemplatedUserControl
    {
        public ImporterUi(ImporterUiViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }

    }
}
