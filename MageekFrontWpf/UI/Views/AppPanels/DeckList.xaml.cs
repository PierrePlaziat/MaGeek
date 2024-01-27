using CommunityToolkit.Mvvm.Messaging;
using MageekFrontWpf.AppValues;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels.AppPanels;
using MageekService.Data.Collection.Entities;
using System.Windows.Controls;

namespace MageekFrontWpf.UI.Views.AppPanels
{

    public partial class DeckList : BaseUserControl
    {

        public DeckList(DeckListViewModel vm)
        {
            DataContext = vm;
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

    }

}
