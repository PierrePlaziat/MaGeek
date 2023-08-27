#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MageekSdk.MtgSqlive.Entities
{
    public class CardForeignData
    {
        public string? FaceName { get; set; }
        public string? FlavorText { get; set; }
        [Key, Column(Order = 1)]
        public string Language { get; set; }
        public int? MultiverseId { get; set; }
        public string Name { get; set; }
        public string? Text { get; set; }
        public string? Type { get; set; }
        [Key, Column(Order = 0)]
        public string Uuid { get; set; }
    }
}
