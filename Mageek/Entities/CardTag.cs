using MaGeek.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace MaGeek.Entities
{
    public class CardTag
    {

        [Key]
        public int Id { get; set; }
        public string Tag { get; set; }
        public string CardId { get; set; }
        public virtual MagicCard Card { get; set; }

        public CardTag() {}

        public CardTag(string tag, MagicCard card)
        {
            this.Tag = tag;
            this.CardId = card.CardId;
            this.Card = card;
        }

    }

}
