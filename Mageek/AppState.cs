using MaGeek.Data.Entities;
using MaGeek.Events;
using System;
using System.Linq;

namespace MaGeek
{
    public class AppState
    {

        #region Change Lang Event

        public string GetForeignLanguage()
        {
            var p = App.Database.Params.Where(x => x.ParamName == "ForeignLanguage");
            if (p.Any())
            {
                return p.FirstOrDefault().ParamValue;
            }
            else
            {
                App.Database.Params.Add(new Entities.Param() { ParamValue = "French", ParamName = "ForeignLanguage" });
                App.Database.SaveChanges();
                return "French";
            }
        }
        public void SetForeignLanguage(string value)
        {
            var p = App.Database.Params.Where(x => x.ParamName == "ForeignLanguage");
            if (p.Any()) App.Database.Params.Remove(p.FirstOrDefault());
            App.Database.Params.Add(new Entities.Param() { ParamValue = value, ParamName = "ForeignLanguage" });
            App.Database.SaveChanges();
            App.Restart();
        }

        #endregion

        #region Deck Focis Event

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

        #region DECK VIEW FLASH

        public void ModifDeck()
        {
            RaiseModifDeck(new DeckModifEventArgs());
        }
        public delegate void DeckModifEventHandler(object sender, DeckModifEventArgs args);
        public event DeckModifEventHandler RaiseDeckModif;
        protected virtual void RaiseModifDeck(DeckModifEventArgs e)
        {
            DeckModifEventHandler raiseEvent = RaiseDeckModif;
            if (raiseEvent != null) raiseEvent(this, e);
        }

        #endregion

        #endregion

        #region CardFocusEvent

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

        #region Collec Modif Event

        public void ModifCollec()
        {
            RaiseCollecModif(new CollecEventArgs());
        }
        public delegate void CollecEventHandler(object sender, CollecEventArgs args);
        public event CollecEventHandler RaiseCollec;
        protected virtual void RaiseCollecModif(CollecEventArgs e)
        {
            CollecEventHandler raiseEvent = RaiseCollec;
            if (raiseEvent != null) raiseEvent(this, e);
        }

        #endregion

    }

}
