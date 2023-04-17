using System.ComponentModel.DataAnnotations;

namespace MaGeek.AppData.Entities
{

    public class CardValue
    {
        [Key]
        public string MultiverseId { get; set; }
        public string ValueEur { get; set; }
        public string ValueUsd { get; set; }
        public int EdhRecRank { get; set; }
    }

}
