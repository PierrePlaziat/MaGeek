using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels.AppPanels;
using MageekCore.Data.Collection.Entities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Messaging;
using MageekFrontWpf.Framework.AppValues;
using System;
using MageekCore.Data;

namespace MageekFrontWpf.UI.Views.AppPanels
{

    public partial class CardInspector : BaseUserControl
    {
        private MageekCore.MageekService mageek;
        private CardInspectorViewModel vm;

        public CardInspector(CardInspectorViewModel vm,MageekCore.MageekService mageek)
        {
            this.mageek = mageek;
            this.vm = vm;
            DataContext = vm;
            InitializeComponent();
        }

        private async void NewTag_KeyUp(object sender, KeyEventArgs e)
        {
            bool found = false;
            var border = (resultStack.Parent as ScrollViewer).Parent as Border;
            var data = await mageek.GetTags();
            string query = (sender as TextBox).Text;
            if (query.Length == 0)
            {
                resultStack.Children.Clear();
                border.Visibility = Visibility.Collapsed;
            }
            else
            {
                border.Visibility = Visibility.Visible;
            }
            resultStack.Children.Clear();
            foreach (var obj in data)
            {
                if (obj != null && obj.TagContent.ToLower().StartsWith(query.ToLower()))
                {
                    AddItem(obj.TagContent);
                    found = true;
                }
            }
            if (!found)
            {
                resultStack.Children.Add(new TextBlock() { Text = "No results found." });
            }
        }

        private void AddItem(string text)
        {
            TextBlock block = new()
            {
                Text = text,
                Margin = new Thickness(2, 3, 2, 3),
                //Cursor = Cursors.Hand
            };
            block.MouseLeftButtonUp += (sender, e) =>
            {
                NewTag.Text = (sender as TextBlock).Text;
            };
            block.MouseEnter += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.Gray;
            };
            block.MouseLeave += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.Transparent;
            };
            resultStack.Children.Add(block);
        }

        private void NewTag_LostFocus(object sender, RoutedEventArgs e)
        {
            sugestions.Visibility = Visibility.Collapsed;
        }

        private void SelectionChanged(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ListView sendedBy = (sender as ListView);
            if (sendedBy.SelectedItem is DeckCard cardRel) vm.Reload(cardRel.CardUuid).ConfigureAwait(false);
            sendedBy.UnselectAll();
        }

        private void ScrollViewer_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer s = (ScrollViewer)sender;
            s.ScrollToVerticalOffset(s.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void GoToRelated(object sender, MouseButtonEventArgs e)
        {
            //TODO
            //vm.GoToRelatedCommand(vm.SelectedUuid);
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            Grid item = (Grid)sender;
            var data = e.Data.GetData(typeof(string)) as string;
            vm.Reload(data).ConfigureAwait(false);
        }

        private void VariantListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var DragSource = (ListView)sender;
            if (DragSource == null) return;

            object data = GetDataFromListBox(DragSource, e.GetPosition(DragSource));
            if (data != null)
                DragDrop.DoDragDrop(DragSource, data, DragDropEffects.Move);
        }

        private static object GetDataFromListBox(ListView source, Point point)
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
                        return ((CardVariant)data).Card.Uuid;
                }
            }
            return null;
        }

    }

}
