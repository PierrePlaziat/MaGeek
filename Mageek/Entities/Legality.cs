using System.ComponentModel.DataAnnotations;

namespace MaGeek.Entities
{

    public class Legality
    {
     
        [Key]
        public int Id { get; set; }
        public string MultiverseId { get; set; }
        public string LastUpdate { get; set; }
        public string Format { get; set; }
        public string IsLegal { get; set; }
    }

}
