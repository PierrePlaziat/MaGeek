using MaGeek.AppData.Entities;
using System.Collections.Generic;

namespace MaGeek.AppFramework
{

    /// <summary>
    /// Singleton
    /// </summary>
    public class AppState
    {

        #region Attributes

        public Queue<string> OutputMessages { get; set; } = new Queue<string>() {};
        const int maxLog = 30;

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
            LogMessage("Welcome");
        }

        #endregion

        #region Methods

        public void LogMessage(string message)
        {
            OutputMessages.Enqueue(message);
            if (OutputMessages.Count>maxLog) OutputMessages.Dequeue();
        }

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
