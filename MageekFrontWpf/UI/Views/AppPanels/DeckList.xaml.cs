using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels;
using System.Windows.Controls;

namespace MaGeek.UI
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
            if (deck != null) events.RaiseDeckSelect(deck);
        }

        private void Decklistbox_SelectionChanged(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (decklistbox.SelectedItem is Deck deck) events.RaiseDeckSelect(deck.DeckId);
        }

    }

}
