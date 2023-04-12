using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace MaGeek.AppData.Entities
{
    public class MagicDeck
    {

        #region Entity

        [Key]
        public int DeckId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string DeckColors { get; set; } //  { return App.Biz.Utils.DeckColors(this); } } 
        public int CardCount { get; set; } 

        public virtual ObservableCollection<CardDeckRelation> CardRelations { get; set; }

        #endregion

        #region CTOR

        public MagicDeck() { } // EF needs

        public MagicDeck(string deckTitle)
        {
            Title = deckTitle;
            CardRelations = new ObservableCollection<CardDeckRelation>();
        }

        #endregion

    }
}
