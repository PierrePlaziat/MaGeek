using MaGeek.Data.Entities;
using MaGeek.Events;

namespace MaGeek
{
    public class AppState
    {

        #region DECK FOCUS GESTION

        private MagicDeck selectedDeck = null;
        public MagicDeck SelectedDeck { get { return selectedDeck; } }
        public void SelectDeck(MagicDeck deck)
        {
            selectedDeck = deck;
            RaiseDeckSelect(new SelectDeckEventArgs(deck));
        }
        public delegate void DeckEventHandler(object sender, SelectDeckEventArgs args);
        public event DeckEventHandler RaiseSelectDeck;
        protected virtual void RaiseDeckSelect(SelectDeckEventArgs e)
        {
            DeckEventHandler raiseEvent = RaiseSelectDeck;
            if (raiseEvent != null) raiseEvent(this, e);
        }

        #endregion

        #region CARD FOCUS GESTION

        private MagicCard selectedCard = null;
        public MagicCard SelectedCard { get { return selectedCard; } }
        internal void SelectCard(MagicCard card)
        {
            selectedCard = card;
            RaiseCardSelect(new SelectCardEventArgs(card));
        }
        public delegate void CardEventHandler(object sender, SelectCardEventArgs args);
        public event CardEventHandler RaiseSelectCard;
        protected virtual void RaiseCardSelect(SelectCardEventArgs e)
        {
            CardEventHandler raiseEvent = RaiseSelectCard;
            if (raiseEvent != null) raiseEvent(this, e);
        }

        #endregion

    }

}
