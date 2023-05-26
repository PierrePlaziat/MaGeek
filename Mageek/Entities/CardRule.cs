using ScryfallApi.Client.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MaGeek.Entities
{
    public class CardRule : BaseItem
    {

        [Key]
        public int RuleId { get; set; }
        public string CardId { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("published_at")]
        public string PublicationDate { get; set; }

        [JsonPropertyName("comment")]
        public string Comment { get; set; }

    }
}
