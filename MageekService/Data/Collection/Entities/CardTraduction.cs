#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MageekServices.Data.Collection.Entities
{

    public class CardTraduction
    {
        [Key, Column(Order = 0)]
        public string CardUuid { get; set; }
        [Key, Column(Order = 1)]
        public string Language { get; set; }
        public string Traduction { get; set; }
        public string NormalizedTraduction { get; set; }
    }
}
