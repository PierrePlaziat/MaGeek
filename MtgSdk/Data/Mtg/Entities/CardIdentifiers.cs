#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

using System.ComponentModel.DataAnnotations;

namespace MageekService.Data.Mtg.Entities
{
    public class CardIdentifiers
    {
        public string? CardKingdomEtchedId { get; set; }
        public string? CardKingdomFoilId { get; set; }
        public string? CardKingdomId { get; set; }
        public string? McmId { get; set; }
        public string? McmMetaId { get; set; }
        public string? MtgArenaId { get; set; }
        public string? MtgjsonFoilVersionId { get; set; }
        public string? MtgjsonNonFoilVersionId { get; set; }
        public string MtgjsonV4Id { get; set; }
        public string? MtgoFoilId { get; set; }
        public string? MtgoId { get; set; }
        public string? MultiverseId { get; set; }
        public string ScryfallId { get; set; }
        public string? ScryfallIllustrationId { get; set; }
        public string ScryfallOracleId { get; set; }
        public string? TcgplayerEtchedProductId { get; set; }
        public string? TcgplayerProductId { get; set; }
        [Key]
        public string Uuid { get; set; }
    }
}
