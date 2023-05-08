using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace MaGeek.AppData.Entities
{
    public class Deck
    {

        #region Entity

        [Key]
        public int DeckId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string DeckColors { get; set; } = "";
        public int CardCount { get; set; } 

        public virtual ObservableCollection<DeckCard> CardRelations { get; set; }

        #endregion

        #region CTOR

        public Deck() { } // EF needs

        public Deck(string deckTitle)
        {
            Title = deckTitle;
            CardRelations = new ObservableCollection<DeckCard>();
        }

        #endregion

    }
}
