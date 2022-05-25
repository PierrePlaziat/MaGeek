using MaGeek.Events;
using MtgApiManager.Lib.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace MaGeek.UI
{
    public partial class CardSearcher : Window
    {

        public CardSearcher()
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
            App.AddCardToDeck(e.Card,e.Deck);
            DeckList.Refresh();
            SelectedDeck.CurrentDeck = null;
            SelectedDeck.CurrentDeck = e.Deck;
        }

    }
}
