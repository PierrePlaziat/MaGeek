using System.Windows;
using System;
using MageekDesktopClient.UI.ViewModels.AppPanels;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;
using MageekCore.Data;
using System.Windows.Media;
using PlaziatWpf.Mvvm;
using MageekDesktopClient.Framework;

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
                vm.DoSearch().ConfigureAwait(false);
            }
        }

        private void CardGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var v = CardGrid.SelectedItem as SearchedCards;
            WeakReferenceMessenger.Default.Send(new CardSelectedMessage(v.Card.Uuid));
        }

        private void FillColorFilterCombo()
        {
            ColorComboBox.ItemsSource = Enum.GetValues(typeof(MtgColorFilter));
        }

        private void ContextMenu_Click(object sender, RoutedEventArgs e)
        {
            vm.FilterName = ((MenuItem)e.OriginalSource).Header.ToString();
        }

        //private async void AddToDeck(object sender, RoutedEventArgs e)
        //{
        //    foreach (Cards c in CardGrid.SelectedItems)
        //    {
        //        await MageekService.MageekService.AddCardToDeck(c.Uuid, App.State.SelectedDeck, 1);
        //    }
        //    App.Events.RaiseUpdateDeck();
        //}

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
                        return ((SearchedCards)data).Card.Uuid;
                }
            }
            return null;
        }

    }


}
