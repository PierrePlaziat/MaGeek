using System.ComponentModel.DataAnnotations;

namespace MaGeek.AppData.Entities
{
    public class CardCardRelation
    {
        [Key]
        public int RelationId { get; set; }
        public string Card1Id { get; set; }
        public string Card2Id { get; set; }
        public virtual MagicCard Card1 { get; set; }
        public virtual MagicCard Card2 { get; set; }
        public string LastUpdate { get; set; } = "";

    }

}
