using MageekSdk;
using MageekSdk.Data.Collection.Entities;

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

        private void DoSelectDeck(string deckId)
        {
            selectedDeck = MageekService.GetDeck(deckId).Result;
        }

        private void DoSelectCard(string cardUuid)
        {
            selectedCard = MageekService.FindCard_Ref(cardUuid).Result;
        }

        #endregion

    }

}
