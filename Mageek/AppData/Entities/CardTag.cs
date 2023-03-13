using System.ComponentModel.DataAnnotations;

namespace MaGeek.AppData.Entities
{
    public class CardTag
    {

        [Key]
        public int Id { get; set; }
        public string Tag { get; set; }
        public string CardId { get; set; }
        public virtual MagicCard Card { get; set; }

        public CardTag() { }

        public CardTag(string tag, MagicCard card)
        {
            Tag = tag;
            CardId = card.CardId;
            Card = card;
        }

    }

}
