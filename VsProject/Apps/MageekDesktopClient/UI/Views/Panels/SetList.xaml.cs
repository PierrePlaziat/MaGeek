using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MageekCore.Data;
using MageekCore.Data.Mtg.Entities;
using MageekDesktopClient.UI.ViewModels.AppPanels;
using PlaziatWpf.Mvvm;

namespace MageekDesktopClient.UI.Views.AppPanels
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
            //vm.SelectSet(((ListView)sender).SelectedItem as Sets);
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        }

        private Point _startPoint;
        private object data;
        private DataGrid lv;
        private void UIElement_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
            lv = (DataGrid)sender;
            data = GetDataFromListBox(lv, e.GetPosition(lv));
        }
        private void UIElement_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Point currentPosition = e.GetPosition(null);
                    Vector diff = _startPoint - currentPosition;
                    if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                        Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                    {
                        DragDrop.DoDragDrop(lv, data, DragDropEffects.Move);
                    }
                }
            }
            catch { }
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
                        return ((SearchedCards)data).Card.Uuid;
                }
            }
            return null;
        }

        private void CardGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var v = (DataGrid)sender;
            if (v.SelectedItem == null) return;
            var vv = v.SelectedItem as SearchedCards;
            vm.SelectCard(vv.CardUuid);
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            vm.SelectSet(((ListView)sender).SelectedItem as Sets);
        }
    }

}
