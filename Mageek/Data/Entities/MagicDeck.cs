using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MaGeek.Data.Entities
{
    public class MagicDeck
    {

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

        [Key]
        public string Name { get; set; }
        public virtual ICollection<MagicCard> Cards { get; set; }


        public int CardCount { 
            get {
                if (Cards != null) return Cards.Count;
                else return -1;
            } 
        }

    }
}
