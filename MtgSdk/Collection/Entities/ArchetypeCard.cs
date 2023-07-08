using System.ComponentModel.DataAnnotations;

namespace MageekSdk.Collection.Entities
{
    public class ArchetypeCard
    {
        [Key]
        public string CardUuid { get; set; }
        public string ArchetypeId { get; set; }
    }
}
