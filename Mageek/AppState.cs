using MaGeek.Data.Entities;
using MaGeek.Events;
using System.Linq;

namespace MaGeek
{
    public class AppState
    {

        public string ForeignLanguage
        {
            get {
                var p = App.database.Params.Where(x => x.ParamName == "ForeignLanguage");
                if (p.Any())
                {
                    return p.FirstOrDefault().ParamValue;
                }
                else
                {
                    App.database.Params.Add(new Entities.Param() { ParamValue = "French", ParamName = "ForeignLanguage" });
                    App.database.SaveChanges();
                    return "French";
                }
            }
            set {
                if (value == null) value = "French";
                var p = App.database.Params.Where(x => x.ParamName == "ForeignLanguage");
                if (p.Any())
                {
                    App.database.Params.Remove(p.FirstOrDefault());
                }
                App.database.Params.Add(new Entities.Param() { ParamValue = value, ParamName = "ForeignLanguage" });
                App.database.SaveChanges();
            }
        }

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

    }

}
