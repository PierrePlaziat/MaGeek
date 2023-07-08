using System.ComponentModel.DataAnnotations;

namespace MageekSdk.Collection.Entities
{
    public class PriceLine
    {
        [Key]
        public string cardUuid { get; set; }
        public string LastUpdate { get; set; }
        public decimal? PriceEur { get; set; }
        public decimal? PriceUsd { get; set; }
        public int EdhrecScore { get; set; }
    }
}