using System.Windows;
using System.Windows.Controls;
using MageekCore.Data;
using System.Windows.Media;
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

        private void CardGrid_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var DragSource = (DataGrid)sender;
            if (DragSource == null) return;

            object data = GetDataFromListBox(DragSource, e.GetPosition(DragSource));
            if (data != null)
                DragDrop.DoDragDrop(DragSource, data, DragDropEffects.Move);
        }

        private static object GetDataFromListBox(DataGrid source, Point point)
        {
            if (source.InputHitTest(point) is UIElement element)
            {
                object data = DependencyProperty.UnsetValue;
                while (data == DependencyProperty.UnsetValue)
                {
                    data = source.ItemContainerGenerator.ItemFromContainer(element);
                    element = VisualTreeHelper.GetParent(element) as UIElement;

                    if (element == source)
                        return null;

                    if (data != DependencyProperty.UnsetValue)
                        return ((Cards)data).Uuid;
                }
            }
            return null;
        }

    }

}
