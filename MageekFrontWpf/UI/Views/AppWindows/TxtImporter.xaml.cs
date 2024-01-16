using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels;
using System.Windows;

namespace MaGeek.UI
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
