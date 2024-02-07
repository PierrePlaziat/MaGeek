#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

using System.ComponentModel.DataAnnotations;

namespace MageekServices.Data.Collection.Entities
{
    public class PriceLine
    {
        [Key]
        public string CardUuid { get; set; }
        public string LastUpdate { get; set; }
        public decimal? PriceEur { get; set; }
        public decimal? PriceUsd { get; set; }
        public int EdhrecScore { get; set; }
    }
}