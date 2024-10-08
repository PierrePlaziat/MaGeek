using System.Windows;
using System;
using MageekDesktopClient.UI.ViewModels.AppPanels;
using System.Windows.Controls;
using MageekCore.Data;
using System.Windows.Media;
using PlaziatWpf.Mvvm;
using System.Windows.Input;

namespace MageekDesktopClient.UI.Views.AppPanels
{


    public partial class CardSearcher : BaseUserControl
    {
        CardSearcherViewModel vm;
        public CardSearcher(CardSearcherViewModel vm)
        {
            this.vm = vm;
            DataContext = vm;
            InitializeComponent();
            FillColorFilterCombo();
        }

        private void FilterName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                e.Handled = true;
                var binding = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
                binding.UpdateSource();
                vm.Search().ConfigureAwait(false);
            }
        }

        private void FillColorFilterCombo()
        {
            ColorComboBox.ItemsSource = Enum.GetValues(typeof(MtgColorFilter));
        }

        private void ContextMenu_Click(object sender, RoutedEventArgs e)
        {
            vm.FilterName = ((MenuItem)e.OriginalSource).Header.ToString();
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
                        return ((SearchedCards)data).CardUuid;
                }
            }
            return null;
        }

    }


}
