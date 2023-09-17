#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

using System.ComponentModel.DataAnnotations;

namespace MageekSdk.MtgSqlive.Entities
{
    public class Tokens
    {
        public string Artist { get; set; }
        public string? AsciiName { get; set; }
        public string Availability { get; set; }
        public string? BoosterTypes { get; set; }
        public string BorderColor { get; set; }
        public string? ColorIdentity { get; set; }
        public string? Colors { get; set; }
        public float? EdhrecSaltiness { get; set; }
        public string? FaceName { get; set; }
        public string Finishes { get; set; }
        public string? FlavorText { get; set; }
        public string? FrameEffects { get; set; }
        public string FrameVersion { get; set; }
        public bool HasFoil { get; set; }
        public bool HasNonFoil { get; set; }
        public bool? IsFullArt { get; set; }
        public bool? IsFunny { get; set; }
        public bool? IsPromo { get; set; }
        public bool? IsReprint { get; set; }
        public bool? IsTextless { get; set; }
        public string? Keywords { get; set; }
        public string Language { get; set; }
        public string Layout { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string? Orientation { get; set; }
        public string? OriginalText { get; set; }
        public string? OriginalType { get; set; }
        public string? OtherFaceIds { get; set; }
        public string? Power { get; set; }
        public string? PromoTypes { get; set; }
        public string? RelatedCards { get; set; }
        public string? reverseRelated{ get; set; }
        public string? SecurityStamp { get; set; }
        public string SetCode { get; set; }
        public string? Side { get; set; }
        public string? Signature { get; set; }
        public string Subtypes { get; set; }
        public string? Supertypes { get; set; }
        public string? Text { get; set; }
        public string? Toughness { get; set; }
        public string Type { get; set; }
        public string Types { get; set; }
        [Key]
        public string Uuid { get; set; }
        public string? Watermark { get; set; }
    }
}
