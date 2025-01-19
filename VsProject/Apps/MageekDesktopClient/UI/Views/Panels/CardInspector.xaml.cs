using MageekDesktopClient.UI.ViewModels.AppPanels;
using MageekCore.Data.Collection.Entities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MageekCore.Data;
using MageekCore.Services;
using PlaziatWpf.Mvvm;
using PlaziatWpf.Services;
using System;

namespace MageekDesktopClient.UI.Views.AppPanels
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

        //TODO restore
        //private async void NewTag_KeyUp(object sender, KeyEventArgs e)
        //{
        //    bool found = false;
        //    var border = (resultStack.Parent as ScrollViewer).Parent as Border;
        //    var data = await mageek.Tags_All(session.UserName);
        //    string query = (sender as TextBox).Text;
        //    if (query.Length == 0)
        //    {
        //        resultStack.Children.Clear();
        //        border.Visibility = Visibility.Collapsed;
        //    }
        //    else
        //    {
        //        border.Visibility = Visibility.Visible;
        //    }
        //    resultStack.Children.Clear();
        //    foreach (var obj in data)
        //    {
        //        if (obj != null && obj.ToLower().StartsWith(query.ToLower()))
        //        {
        //            AddItem(obj);
        //            found = true;
        //        }
        //    }
        //    if (!found)
        //    {
        //        resultStack.Children.Add(new TextBlock() { Text = "No results found." });
        //    }
        //}

        //private void AddItem(string text)
        //{
        //    TextBlock block = new()
        //    {
        //        Text = text,
        //        Margin = new Thickness(2, 3, 2, 3),
        //        //Cursor = Cursors.Hand
        //    };
        //    block.MouseLeftButtonUp += (sender, e) =>
        //    {
        //        //NewTag.Text = (sender as TextBlock).Text;//TODO restore
        //    };
        //    block.MouseEnter += (sender, e) =>
        //    {
        //        TextBlock b = sender as TextBlock;
        //        b.Background = Brushes.Gray;
        //    };
        //    block.MouseLeave += (sender, e) =>
        //    {
        //        TextBlock b = sender as TextBlock;
        //        b.Background = Brushes.Transparent;
        //    };
        //    resultStack.Children.Add(block);
        //}

        //private void NewTag_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    sugestions.Visibility = Visibility.Collapsed;
        //}

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

        private Point _startPoint;
        private object data;
        private ListView lv;
        private void UIElement_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
            lv = (ListView)sender;
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
            catch{ }
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

        private void VariantListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView sendedBy = (sender as ListView);
            var v = sendedBy.SelectedItem as CardVariant;
            vm.Reload(v.Card.Uuid).ConfigureAwait(false);
        }
    }

}
