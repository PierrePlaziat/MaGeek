#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

using System.ComponentModel.DataAnnotations;

namespace MageekSdk.Data.Mtg.Entities
{
    public class CardRulings
    {
        public string Date { get; set; }
        public string Text { get; set; }
        [Key]
        public string Uuid { get; set; }
    }
}
