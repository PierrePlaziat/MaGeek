using System.ComponentModel.DataAnnotations;

namespace MaGeek.AppData.Entities
{

    public class CardValue
    {
        [Key]
        public string MultiverseId { get; set; }
        public string LastUpdate { get; set; }
        public float ValueEur { get; set; }
        public float ValueUsd { get; set; }
        public int EdhRank { get; set; }
    }

}
