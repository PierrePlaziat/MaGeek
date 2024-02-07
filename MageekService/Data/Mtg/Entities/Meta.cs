#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

using System.ComponentModel.DataAnnotations;

namespace MageekServices.Data.Mtg.Entities
{
    public class Meta
    {
        public string Date { get; set; }
        [Key]
        public string Version { get; set; }
    }
}
