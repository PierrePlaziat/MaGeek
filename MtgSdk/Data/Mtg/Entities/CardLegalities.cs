#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

using System.ComponentModel.DataAnnotations;

namespace MageekService.Data.Mtg.Entities
{
    public class CardLegalities
    {
        public string? Alchemy { get; set; }
        public string? Brawl { get; set; }
        public string? Commander { get; set; }
        public string? Duel { get; set; }
        public string? Explorer { get; set; }
        public string? Future { get; set; }
        public string? Gladiator { get; set; }
        public string? Historic { get; set; }
        public string? Historicbrawl { get; set; }
        public string? Legacy { get; set; }
        public string? Modern { get; set; }
        public string? Oathbreaker { get; set; }
        public string? Oldschool { get; set; }
        public string? Pauper { get; set; }
        public string? Paupercommander { get; set; }
        public string? Penny { get; set; }
        public string? Pioneer { get; set; }
        public string? Predh { get; set; }
        public string? Premodern { get; set; }
        public string? Standard { get; set; }
        [Key]
        public string Uuid { get; set; }
        public string? Vintage { get; set; }
    }
}
