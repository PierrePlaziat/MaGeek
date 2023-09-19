#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

using MtgSqliveSdk;
using System.ComponentModel.DataAnnotations;

namespace MageekSdk.MtgSqlive.Entities
{

    public class Cards
    {
        public int Collected
        {
            get 
            {
                return Mageek.CollectedCard_HowMany(Uuid, true).Result;
            }
        }

        public string CardForeignName
        {
            get 
            {
                string lang = "French";
                return Mageek.GetTraduction(Uuid, lang).Result;
            }
        }

        public Sets Set 
        { 
            get 
            {
                return Mageek.RetrieveSet(SetCode).Result;
            } 
        }

        public string? Artist { get; set; }
        public string? AsciiName { get; set; }
        public string? AttractionLights { get; set; }
        public string Availability { get; set; }
        public string? BoosterTypes { get; set; }
        public string BorderColor { get; set; }
        public string? CardParts { get; set; }
        public string ColorIdentity { get; set; }
        public string? ColorIndicator { get; set; }
        public string Colors { get; set; }
        public string? Defense { get; set; }
        public string? DuelDeck { get; set; }
        public int? EdhrecRank { get; set; }
        public float? EdhrecSaltiness { get; set; }
        public float? FaceConvertedManaCost { get; set; }
        public string? FaceFlavorName { get; set; }
        public float? FaceManaValue { get; set; }
        public string? FaceName { get; set; }
        public string Finishes { get; set; }
        public string? FlavorName { get; set; }
        public string? FlavorText { get; set; }
        public string? FrameEffects { get; set; }
        public string FrameVersion { get; set; }
        public string? Hand { get; set; }
        public bool? HasAlternativeDeckLimit { get; set; }
        public bool? HasContentWarning { get; set; }
        public bool HasFoil { get; set; }
        public bool HasNonFoil { get; set; }
        public bool? IsAlternative { get; set; }
        public bool? IsFullArt { get; set; }
        public bool? IsFunny { get; set; }
        public bool? IsOnlineOnly { get; set; }
        public bool? IsOversized { get; set; }
        public bool? IsPromo { get; set; }
        public bool? IsRebalanced { get; set; }
        public bool? IsReprint { get; set; }
        public bool? IsReserved { get; set; }
        public bool? IsStarter { get; set; }
        public bool? IsStorySpotlight { get; set; }
        public bool? IsTextless { get; set; }
        public bool? IsTimeshifted { get; set; }
        public string? Keywords { get; set; }
        public string Language { get; set; }
        public string Layout { get; set; }
        public string? LeadershipSkills { get; set; }
        public string? Life { get; set; }
        public string? Loyalty { get; set; }
        public string? ManaCost { get; set; }
        public float ManaValue { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string? OriginalPrintings { get; set; }
        public string? OriginalReleaseDate { get; set; }
        public string? OriginalText { get; set; }
        public string? OriginalType { get; set; }
        public string? OtherFaceIds { get; set; }
        public string? Power { get; set; }
        public string Printings { get; set; }
        public string? PromoTypes { get; set; }
        public string Rarity { get; set; }
        public string? RebalancedPrintings { get; set; }
        public string? RelatedCards { get; set; }
        public string? SecurityStamp { get; set; }
        public string SetCode { get; set; }
        public string? Side { get; set; }
        public string? Signature { get; set; }
        public string? Subsets { get; set; }
        public string Subtypes { get; set; }
        public string Supertypes { get; set; }
        public string? Text { get; set; }
        public string? Toughness { get; set; }
        public string Type { get; set; }
        public string Types { get; set; }
        [Key]
        public string Uuid { get; set; }
        public string? Variations { get; set; }
        public string? Watermark { get; set; }
    }
}
