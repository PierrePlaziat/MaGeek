#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

using System.ComponentModel.DataAnnotations;

namespace MageekSdk.Data.Collection.Entities
{
    public class FavVariant
    {
        [Key]
        public string ArchetypeId { get; set; }
        public string FavUuid { get; set; }
    }
}