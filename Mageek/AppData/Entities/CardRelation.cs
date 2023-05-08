using System.ComponentModel.DataAnnotations;

namespace MaGeek.AppData.Entities
{
    public class CardRelation
    {
        [Key]
        public int RelationId { get; set; }
        public string Card1Id { get; set; }
        public string Card2Id { get; set; }
        public string LastUpdate { get; set; } = "";
        public string RelationType { get; set; } = "";

        public virtual CardModel Card1 { get; set; }
        public virtual CardModel Card2 { get; set; }

    }

}
