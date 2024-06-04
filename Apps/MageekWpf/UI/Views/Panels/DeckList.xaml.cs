using MageekFrontWpf.UI.ViewModels.AppPanels;
using MageekCore.Data.Collection.Entities;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows;
using PlaziatWpf.Mvvm;
using MageekFrontWpf.Framework;

namespace MageekFrontWpf.UI.Views.AppPanels
{

    public partial class DeckList : BaseUserControl
    {
        private DeckListViewModel vm;

        public DeckList(DeckListViewModel vm)
        {
            DataContext = vm;
            this.vm = vm;
            InitializeComponent();
        }

        private void Decklistbox_SelectionChanged(object sender, MouseButtonEventArgs e)
        {
            Deck deck = (Deck)decklistbox.SelectedItem;
            vm.SelectDeck(deck.DeckId).ConfigureAwait(false);
        }

        private void MenuItem_OpenDeckClick(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            vm.SelectDeck((string)item.CommandParameter).ConfigureAwait(false); 
        }

        private void MenuItem_CreateDeckClick(object sender, RoutedEventArgs e)
        {
            vm.AddDeck().ConfigureAwait(false);
        }

        private void MenuItem_RenameDeckClick(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            vm.RenameDeck((string)item.CommandParameter).ConfigureAwait(false);
        }

        private void MenuItem_DuplicateDeckClick(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            vm.DuplicateDeck((string)item.CommandParameter).ConfigureAwait(false);
        }

        private void MenuItem_ListDeckClick(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            vm.GetAsTxtList((string)item.CommandParameter).ConfigureAwait(false);
        }

        private void MenuItem_EstimateDeckClick(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            vm.EstimateDeckPrice((string)item.CommandParameter).ConfigureAwait(false);
        }

        private void MenuItem_DeleteDeckClick(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            vm.DeleteDeck((string)item.CommandParameter).ConfigureAwait(false);
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            Grid item = (Grid)sender;
            var data = e.Data.GetData(typeof(string)) as string;
            string deckId = ((Deck)item.DataContext).DeckId;
            vm.SelectDeck(deckId).ConfigureAwait(true);
            WeakReferenceMessenger.Default.Send(
                new AddCardToDeckMessage(new Tuple<string,string>(deckId, data))
            );
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab || e.Key == Key.Escape)
            {
                TextBox tBox = (TextBox)sender;
                DependencyProperty prop = TextBox.TextProperty;
                BindingExpression binding = BindingOperations.GetBindingExpression(tBox, prop);
                if (binding != null) { binding.UpdateSource(); }
                vm.Reload().ConfigureAwait(false);
            }
        }

        private void ButtonDeleteFilter_Click(object sender, RoutedEventArgs e)
        {
            TB.Text = string.Empty;
            DependencyProperty prop = TextBox.TextProperty;
            BindingExpression binding = BindingOperations.GetBindingExpression(TB, prop);
            if (binding != null) { binding.UpdateSource(); }
            vm.Reload().ConfigureAwait(false);
        }
    }

}
