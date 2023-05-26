using System.ComponentModel.DataAnnotations;

namespace MaGeek.AppBusiness.Entities
{
    public class CardTag
    {

        [Key]
        public int Id { get; set; }
        public string Tag { get; set; }
        public string CardId { get; set; }
        public virtual CardModel Card { get; set; }

        public CardTag() { }

        public CardTag(string tag, CardModel card)
        {
            Tag = tag;
            CardId = card.CardId;
            Card = card;
        }

    }

}
