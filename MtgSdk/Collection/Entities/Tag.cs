#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

using System.ComponentModel.DataAnnotations;

namespace MageekSdk.Collection.Entities
{
    public class Tag
    {
        [Key]
        public int TagId { get; set; }
        public string TagContent { get; set; }
        public string ArchetypeId { get; set; }
    }
}
