using MaGeek.Data.Entities;

namespace MaGeek
{
    public class AppState
    {


        private MagicCard selectedCard = null;
        public MagicCard SelectedCard { get { return selectedCard; } }

        private MagicDeck selectedDeck = null;
        public MagicDeck SelectedDeck { get { return selectedDeck; } }

        #region Events

        // Layout Action 

        public delegate void LayoutActionHandler(LayoutEventType Type);
        public event LayoutActionHandler LayoutActionEvent;
        public virtual void RaiseLayoutAction(LayoutEventType Type) { LayoutActionEvent(Type); }

        // Card Selected

        public delegate void CardSelectedHandler(MagicCard Card);
        public event CardSelectedHandler CardSelectedEvent;
        public virtual void RaiseCardSelected(MagicCard Card) { selectedCard = Card; CardSelectedEvent(Card); }

        // Deck Selected

        public delegate void SelectDeckHandler(MagicDeck deck);
        public event SelectDeckHandler SelectDeckEvent;
        public virtual void RaiseDeckSelect(MagicDeck deck) { selectedDeck = deck; SelectDeckEvent(deck); }

        // Update Card Collec 

        public delegate void UpdateCardCollecdHandler();
        public event UpdateCardCollecdHandler UpdateCardCollecEvent;
        public virtual void RaiseUpdateCardCollec() { UpdateCardCollecEvent(); }

        // Update Deck 

        public delegate void UpdateDeckHandler();
        public event UpdateDeckHandler UpdateDeckEvent;
        public virtual void RaiseUpdateDeck() { UpdateDeckEvent(); }


        public delegate void UpdateDeckListHandler();
        public event UpdateDeckListHandler UpdateDeckListEvent;
        public virtual void RaiseUpdateDeckList() { UpdateDeckListEvent(); }

        #endregion

    }

}
