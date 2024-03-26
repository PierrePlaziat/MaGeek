#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

using System.ComponentModel.DataAnnotations;

namespace MageekCore.Data.Collection.Entities
{
    public class PriceLine
    {

        [Key]
        public string CardUuid { get; set; }
        public string? PriceEurAccrossTime { get; set; }
        public string? PriceUsdAccrossTime { get; set; }
        public float? LastPriceEur { get; set; }
        public float? LastPriceUsd { get; set; }

    }
}