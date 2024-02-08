using CommunityToolkit.Mvvm.Messaging;
using MageekFrontWpf.AppValues;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels.AppPanels;
using MageekServices.Data.Collection.Entities;
using System;
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

        private void decklistbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var deck = decklistbox.SelectedItem as Deck;
                WeakReferenceMessenger.Default.Send(new UpdateDeckMessage(deck.DeckId));
        }

        private void Decklistbox_SelectionChanged(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (decklistbox.SelectedItem is Deck deck)
                WeakReferenceMessenger.Default.Send(new UpdateDeckMessage(deck.DeckId));
        }

        private void MenuItem_OpenDeckClick(object sender, System.Windows.RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            vm.SelectDeck((string)item.CommandParameter).ConfigureAwait(false); 
        }

    }

}
