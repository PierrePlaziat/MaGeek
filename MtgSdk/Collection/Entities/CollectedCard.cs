using System.ComponentModel.DataAnnotations;

namespace MageekSdk.Collection.Entities
{
    public class CollectedCard
    {
        [Key]
        public string CardUuid { get; set; }
        public int Collected { get; set; }
    }
}