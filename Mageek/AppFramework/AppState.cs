using MaGeek.AppData.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MaGeek.AppFramework
{

    /// <summary>
    /// Singleton
    /// </summary>
    public class AppState
    {

        #region Attributes

        public ConcurrentQueue<string> OutputMessages { get; } = new ConcurrentQueue<string>() {};
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
        }

        #endregion

        #region Methods

        public void LogMessage(string message)
        {
            OutputMessages.Enqueue(message);
            string s;
            if (OutputMessages.Count>maxLog) OutputMessages.TryDequeue(out s);
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
