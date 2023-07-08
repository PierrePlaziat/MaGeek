using System.ComponentModel.DataAnnotations;

namespace MageekSdk.Collection.Entities
{
    public class Tag
    {
        [Key]
        public int TagId { get; set; }
        public string TagContent { get; set; }
        public string ArchetypeId { get; set; }
    }
}
