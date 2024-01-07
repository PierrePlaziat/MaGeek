using MageekFrontWpf.ViewModels;
using System.Windows;

namespace MaGeek.UI
{

    public partial class TxtImporter : Window
    {

        public TxtImporter(TxtImporterViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }

    }

}
