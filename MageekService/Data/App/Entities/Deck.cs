#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

using System.ComponentModel.DataAnnotations;

namespace MageekService.Data.Collection.Entities
{
    public class Deck
    {
        [Key]
        public string DeckId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string DeckColors { get; set; } = "";
        public int CardCount { get; set; }
    }
}
