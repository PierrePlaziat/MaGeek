using MaGeek.Events;
using System.Windows;

namespace MaGeek.UI
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            LocalColec.RaiseSelectCard += HandleCardSelected;
            DeckList.RaiseCustomEvent += HandleDeckSelected;
            CardDetails.RaiseCustomEvent += HandleAddToDeck;
        }

        void HandleCardSelected(object sender, SelectCardEventArgs e)
        {
            CardDetails.SelectedCard = e.Card;
        }

        void HandleDeckSelected(object sender, SelectDeckEventArgs e)
        {
            SelectedDeck.CurrentDeck = e.Deck;
        }

        void HandleAddToDeck(object sender, AddToDeckEventArgs e)
        {
            App.cardManager.AddCardToDeck(e.Card,e.Deck);
            DeckList.Refresh();
            SelectedDeck.CurrentDeck = null;
            SelectedDeck.CurrentDeck = e.Deck;
        }

    }
}
