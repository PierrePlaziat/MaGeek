using MageekFrontWpf.MageekTools.DeckTools;
using MageekFrontWpf.UI.ViewModels;
using PlaziatWpf.Mvvm;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MageekFrontWpf.UI.Views.AppPanels
{

    public partial class DeckContent : BaseUserControl
    {

        private DocumentViewModel vm;

        public DeckContent() {}
        
        public void SetDataContext(DocumentViewModel vm)
        {
            this.vm = vm;
            DataContext = vm;
            InitializeComponent();
        }

        private void SelectionChanged(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ((ListView)sender).UnselectAll();
        }

        private void ButtonLess_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ManipulableDeckEntry entry = ((ManipulableDeckEntry)((Button)sender).CommandParameter);
            vm.LessCard(entry).ConfigureAwait(false);
        }

        private void ButtonMore_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ManipulableDeckEntry entry = ((ManipulableDeckEntry)((Button)sender).CommandParameter);
            vm.MoreCard(entry).ConfigureAwait(false);
        }

        private void SetCommandant(object sender, System.Windows.RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            ManipulableDeckEntry entry = (ManipulableDeckEntry)item.CommandParameter;
            vm.ToCommandant(entry);
        }

        private void ToSide(object sender, System.Windows.RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            ManipulableDeckEntry entry = (ManipulableDeckEntry)item.CommandParameter;
            vm.ToSide(entry);
        }

        private void ToDeck(object sender, System.Windows.RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            ManipulableDeckEntry entry = (ManipulableDeckEntry)item.CommandParameter;
            vm.ToDeck(entry);
        }

        private void ScrollViewer_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            //TODO
        }

        private void Grid_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
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
                        return ((ManipulableDeckEntry)data).Card.Uuid;
                }
            }
            return null;
        }

    }

}
