using System.ComponentModel.DataAnnotations;

namespace MageekSdk.Collection.Entities
{
    public class Deck
    {
        [Key]
        public int DeckId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string DeckColors { get; set; } = "";
        public int CardCount { get; set; }
    }
}
