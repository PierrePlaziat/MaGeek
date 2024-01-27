using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels.AppWindows;

namespace MageekFrontWpf.UI.Views.AppWindows
{

    public partial class ImportWindow : BaseWindow
    {

        public ImportWindow(ImportViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }

    }

}
