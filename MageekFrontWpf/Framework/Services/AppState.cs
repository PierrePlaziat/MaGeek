using MaGeek;
using MageekService;
using MageekService.Data.Collection.Entities;

namespace MageekFrontWpf.Framework.Services
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

        public AppState(AppEvents events)
        {
            events.CardSelectedEvent += DoSelectCard;
            events.SelectDeckEvent += DoSelectDeck;
        }

        #endregion

        #region Methods

        private void DoSelectDeck(string deckId)
        {
            selectedDeck = MageekService.MageekService.GetDeck(deckId).Result;
        }

        private void DoSelectCard(string cardUuid)
        {
            selectedCard = MageekService.MageekService.FindCard_Ref(cardUuid).Result;
        }

        #endregion

    }

}
