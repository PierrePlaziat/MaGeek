using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MaGeek.Data.Entities
{
    public class MagicDeck
    {

        #region Entity

        [Key] 
        public int DeckId { get; set; }
        public string Title { get; set; }
        public virtual ObservableCollection<CardDeckRelation> CardRelations { get; set; }

        #endregion

        #region CTOR

        public MagicDeck() {} // EF needs

        public MagicDeck(string deckTitle)
        {
            Title = deckTitle;
            CardRelations = new ObservableCollection<CardDeckRelation>();
        }

        public MagicDeck(MagicDeck deckToCopy)
        {
            this.Title = deckToCopy.Title+ " - Copie";
            CardRelations = new ObservableCollection<CardDeckRelation>();
            foreach (CardDeckRelation relation in deckToCopy.CardRelations)
            {
                App.CARDS.Utils.AddCardToDeck(relation.Card,this,relation.Quantity,relation.RelationType);
            }
        }

        #endregion

        #region Accessors

        public string DeckColors { get { return App.CARDS.Utils.DeckColors(this); } }
        public int CardCount { get { return App.CARDS.Utils.count_Total(this); } }

        #endregion

    }
}
