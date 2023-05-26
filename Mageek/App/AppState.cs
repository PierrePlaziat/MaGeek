using MaGeek.Entities;

namespace MaGeek
{

    /// <summary>
    /// Singleton
    /// </summary>
    public class AppState
    {

        #region Attributes

        private CardModel selectedCard = null;
        public CardModel SelectedCard { get { return selectedCard; } }

        private Deck selectedDeck = null;
        public Deck SelectedDeck { get { return selectedDeck; } }

        #endregion

        #region CTOR

        public AppState()
        {
            App.Events.CardSelectedEvent += DoSelectCard;
            App.Events.SelectDeckEvent += DoSelectDeck;
        }

        #endregion

        #region Methods

        private void DoSelectDeck(Deck deck)
        {
            selectedDeck = deck;
        }

        private void DoSelectCard(CardModel Card)
        {
            selectedCard = Card;
        }

        #endregion

    }

}
