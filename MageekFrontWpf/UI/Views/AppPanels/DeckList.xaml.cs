﻿using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels.AppPanels;
using MageekCore.Data.Collection.Entities;
using System.Windows.Controls;

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

        private void Decklistbox_SelectionChanged(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Deck deck = (Deck)decklistbox.SelectedItem;
            vm.SelectDeck(deck.DeckId).ConfigureAwait(false);
        }

        private void MenuItem_OpenDeckClick(object sender, System.Windows.RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            vm.SelectDeck((string)item.CommandParameter).ConfigureAwait(false); 
        }

        private void MenuItem_CreateDeckClick(object sender, System.Windows.RoutedEventArgs e)
        {
            vm.AddDeck().ConfigureAwait(false);
        }

        private void MenuItem_RenameDeckClick(object sender, System.Windows.RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            vm.RenameDeck((string)item.CommandParameter).ConfigureAwait(false);
        }

        private void MenuItem_DuplicateDeckClick(object sender, System.Windows.RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            vm.DuplicateDeck((string)item.CommandParameter).ConfigureAwait(false);
        }

        private void MenuItem_ListDeckClick(object sender, System.Windows.RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            vm.GetAsTxtList((string)item.CommandParameter).ConfigureAwait(false);
        }

        private void MenuItem_EstimateDeckClick(object sender, System.Windows.RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            vm.EstimateDeckPrice((string)item.CommandParameter).ConfigureAwait(false);
        }

        private void MenuItem_DeleteDeckClick(object sender, System.Windows.RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            vm.DeleteDeck((string)item.CommandParameter).ConfigureAwait(false);
        }

    }

}
