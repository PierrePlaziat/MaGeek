using MageekDesktop.UI.ViewModels.AppPanels;
using MageekCore.Data.Collection.Entities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MageekCore.Data;
using MageekCore.Services;
using PlaziatWpf.Mvvm;
using PlaziatWpf.Services;

namespace MageekDesktop.UI.Views.AppPanels
{

    public partial class CardInspector : BaseUserControl
    {
        private IMageekService mageek;
        private CardInspectorViewModel vm;
        private SessionBag session;

        public CardInspector(CardInspectorViewModel vm, IMageekService mageek, SessionBag session)
        {
            this.mageek = mageek;
            this.vm = vm;
            this.session = session;
            DataContext = vm;
            InitializeComponent();
        }

        private async void NewTag_KeyUp(object sender, KeyEventArgs e)
        {
            bool found = false;
            var border = (resultStack.Parent as ScrollViewer).Parent as Border;
            var data = await mageek.Tags_All(session.UserName);
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
                if (obj != null && obj.ToLower().StartsWith(query.ToLower()))
                {
                    AddItem(obj);
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
