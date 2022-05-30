using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MaGeek.Data.Entities
{
    public class MagicDeck
    {

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Commander { get; set; }
        public virtual ICollection<MagicCard> Cards { get; set; } = new List<MagicCard>();

        #region CTOR

        public MagicDeck() {}

        public MagicDeck(string deckTitle)
        {
            Name = deckTitle;
        }

        public MagicDeck(MagicDeck deckToCopy)
        {
            this.Name = deckToCopy.Name + " - Copie";
            Cards = new List<MagicCard>(deckToCopy.Cards);
        }

        #endregion

        #region Accessors

        public int CardCount { 
            get {
                if (Cards != null) return Cards.Count;
                else return -1;
            } 
        }

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
                if (Cards == null) return 0;
                int devotion = 0;
                foreach (var c in Cards) devotion += c.DevotionB;
                return devotion;
            }
        }
        public int DevotionW
        {
            get
            {
                if (Cards == null) return 0;
                int devotion = 0;
                foreach (var c in Cards) devotion += c.DevotionW;
                return devotion;
            }
        }
        public int DevotionU
        {
            get
            {
                if (Cards == null) return 0;
                int devotion = 0;
                foreach (var c in Cards) devotion += c.DevotionU;
                return devotion;
            }
        }
        public int DevotionG
        {
            get
            {
                if (Cards == null) return 0;
                int devotion = 0;
                foreach (var c in Cards) devotion += c.DevotionG;
                return devotion;
            }
        }
        public int DevotionR
        {
            get
            {
                if (Cards == null) return 0;
                int devotion = 0;
                foreach (var c in Cards) devotion += c.DevotionR;
                return devotion;
            }
        }

        #endregion

    }
}
