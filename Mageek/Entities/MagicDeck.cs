using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace MaGeek.Data.Entities
{
    public class MagicDeck
    {

        #region Entity

        [Key] public int DeckId { get; set; }
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
            CardRelations = new ObservableCollection<CardDeckRelation>(deckToCopy.CardRelations);
        }

        #endregion

        // TODO Optimize, probably Move

        #region Accessors

        public int CardCount { 
            get {
                int count = 0;
                if (CardRelations != null)
                {
                    foreach (var card in CardRelations)
                    {
                        count += card.Quantity;
                    }
                }
                return count;
            } 
        }

        #region Colors access

        public string DeckColors { 
            get {
                string retour = "";
                if (DevotionB > 0) retour += "b";
                if (DevotionW > 0) retour += "w";
                if (DevotionU > 0) retour += "u";
                if (DevotionG > 0) retour += "g";
                if (DevotionR > 0) retour += "r";
                return retour; 
            }
        }

        public int DevotionB
        {
            get
            {
                if (CardRelations == null) return 0;
                int devotion = 0;
                foreach (var c in CardRelations) devotion += c.Card.Card.DevotionB * c.Quantity;
                return devotion;
            }
        }
        public int DevotionW
        {
            get
            {
                if (CardRelations == null) return 0;
                int devotion = 0;
                foreach (var c in CardRelations) devotion += c.Card.Card.DevotionW * c.Quantity;
                return devotion;
            }
        }
        public int DevotionU
        {
            get
            {
                if (CardRelations == null) return 0;
                int devotion = 0;
                foreach (var c in CardRelations) devotion += c.Card.Card.DevotionU * c.Quantity;
                return devotion;
            }
        }
        public int DevotionG
        {
            get
            {
                if (CardRelations == null) return 0;
                int devotion = 0;
                foreach (var c in CardRelations) devotion += c.Card.Card.DevotionG * c.Quantity;
                return devotion;
            }
        }
        public int DevotionR
        {
            get
            {
                if (CardRelations == null) return 0;
                int devotion = 0;
                foreach (var c in CardRelations) devotion += c.Card.Card.DevotionR * c.Quantity;
                return devotion;
            }
        }

        #endregion

        #endregion

    }
}
