using System.Windows.Controls;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels;

namespace MaGeek.UI
{
    public partial class SetExplorer : BaseUserControl
    {

        public SetExplorer(SetExplorerViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }

}
