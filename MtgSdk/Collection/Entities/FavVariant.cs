using System.ComponentModel.DataAnnotations;

namespace MageekSdk.Collection.Entities
{
    public class FavVariant
    {
        [Key]
        public string ArchetypeId { get; set; }
        public string FavUuid { get; set; }
    }
}