using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.ViewModels;
using System.Windows;

namespace MaGeek.UI
{

    public partial class TxtImporter : BaseWindow
    {

        public TxtImporter(TxtImporterViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }

    }

}
