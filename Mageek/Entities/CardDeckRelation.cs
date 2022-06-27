using MaGeek.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaGeek.Entities
{

    public class CardDeckRelation
    {


        [Key, Column(Order = 0)]
        public int DeckId { get; set; }
        [Key, Column(Order = 1)]
        public string CardId { get; set; }

        public virtual MagicDeck Deck { get; set; }
        public virtual MagicCardVariant Card { get; set; }

        public int Quantity{ get; set; }

    }

}
