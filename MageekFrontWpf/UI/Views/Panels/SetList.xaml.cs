using System.Windows.Controls;
using MageekCore.Data.Mtg.Entities;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels.AppPanels;

namespace MageekFrontWpf.UI.Views.AppPanels
{
    public partial class SetList : BaseUserControl
    {
        private SetListViewModel vm;

        public SetList(SetListViewModel vm)
        {
            DataContext = vm;
            this.vm = vm;
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            vm.Reload().ConfigureAwait(false);
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            vm.SelectSet(((ListView)sender).SelectedItem as Sets);
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var v = (DataGrid)sender;
            if (v.SelectedItem == null) return;
            vm.SelectCard((v.SelectedItem as Cards));
        }
    }

}
