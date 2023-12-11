#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

using System.ComponentModel.DataAnnotations;

namespace MageekService.Data.Collection.Entities
{
    public class CollectedCard
    {
        [Key]
        public string CardUuid { get; set; }
        public int Collected { get; set; }
    }
}