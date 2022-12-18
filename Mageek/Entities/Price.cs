using System.ComponentModel.DataAnnotations;

namespace MaGeek.Entities
{

    public class Price
    {
        [Key]
        public string MultiverseId { get; set; }
        public string LastUpdate { get; set; }
        public string Value { get; set; }
    }

}
