using MageekSdk.Collection.Entities;
using MtgSqliveSdk;

namespace MaGeek
{

    public class AppState
    {

        #region Attributes

        private ArchetypeCard selectedCard = null;
        public ArchetypeCard SelectedCard { get { return selectedCard; } }

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

        private void DoSelectDeck(int deckId)
        {
            selectedDeck = Mageek.GetDeck(deckId).Result;
        }

        private void DoSelectCard(string cardUuid)
        {
            selectedCard = Mageek.FindCard_Ref(cardUuid).Result;
        }

        #endregion

    }

}
