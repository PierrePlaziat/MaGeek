using MageekSdk.Data.Mtg.Entities;

namespace MageekSdk.Data
{

    /// <summary>
    /// Relations beetween cards incliding tokens, melds and combos
    /// </summary>
    public class CardCardRelation
    {
        public CardCardRelationRole Role { get; set; }
        public Cards Card { get; set; }
        public Tokens Token { get; set; }
    }

    /// <summary>
    /// Scryfall available roles
    /// </summary>
    public enum CardCardRelationRole
    {
        token, meld_part, meld_result, combo_piece
    }

    /// <summary>
    /// Scryfall available images formats
    /// </summary>
    public enum CardImageFormat
    {
        small, large, medium, png, art_crop, border_crop
    }

    /// <summary>
    /// Aggregated into a deck
    /// </summary>
    public struct DeckLine
    {
        public string Uuid;
        public int Quantity;
        public int Relation;
    }


}
