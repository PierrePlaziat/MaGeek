using System.ComponentModel.DataAnnotations;

namespace MaGeek.AppBusiness.Entities
{

    public class CardTraduction
    {
        [Key]
        public int TraductionId { get; set; }
        public string CardId { get; set; }
        public string Language { get; set; }
        public string TraductedName { get; set; }
        public string Normalized { get; set; }

        public virtual CardModel Card { get; set; }
    }

}
