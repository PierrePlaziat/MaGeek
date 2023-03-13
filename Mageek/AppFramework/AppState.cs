using MaGeek.Entities;

namespace MaGeek.AppFramework
{

    /// <summary>
    /// Singleton
    /// </summary>
    public class AppState
    {

        private MagicCard selectedCard = null;
        public MagicCard SelectedCard { get { return selectedCard; } }

        private MagicDeck selectedDeck = null;
        public MagicDeck SelectedDeck { get { return selectedDeck; } }


        public AppState()
        {
            App.Events.CardSelectedEvent += DoSelectCard;
            App.Events.SelectDeckEvent += DoSelectDeck;
        }

        private void DoSelectDeck(MagicDeck deck) { selectedDeck = deck; }
        private void DoSelectCard(MagicCard Card) { selectedCard = Card; }

    }

}
