using MageekCore.Data.Collection.Entities;
using MageekCore.Data.Mtg.Entities;

namespace MageekCore.Data
{

    public class Preco
    {
        public required string Code { get; set; }
        public required string Title { get; set; }
        public required string ReleaseDate { get; set; }
        public required string Kind { get; set; }
        public required List<string> CommanderCardUuids { get; set; }
        public required List<string> MainCardUuids { get; set; }
        public required List<string> SideCardUuids { get; set; }
    }

    public class CardVariant
    {

        public Cards Card { get; set; }
        public Sets Set { get; set; }
        public int Collected { get; set; }
        public PriceLine Price { get; set; }

        public CardVariant(Cards card, Sets set, int collected, PriceLine price)
        {
            Card = card;
            Set = set;
            Collected = collected;
            Price = price;
        }

        public decimal? GetPrice //TODO multi monaie & colors
        {
            get
            {
                if (Price == null) return null;
                if (Price.PriceEur == null) return null;
                return Price.PriceEur;
            }
        }
    }

    public class SearchedCards
    {

        public SearchedCards(Cards card, string translation, int collected)
        {
            Card = card;
            Translation = translation;
            Collected = collected;
        }

        public Cards Card { get; }
        public string Translation { get; }
        public int Collected { get; }

    }

    public enum MageekInitReturn
    {
        Error,
        MtgUpToDate,
        MtgOutdated,
    }

    public enum MageekUpdateReturn
    {
        ErrorDownloading,
        ErrorFetching,
        Success,
    }


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

    public enum MtgColorFilter
    {
        _,//NoFilter,
        X,//Colorless
        W,//White
        B,//Black
        G,//Green
        U,//Blue
        R,//Red
        GW,//SELESNYA
        WU,//AZORIUS
        BU,//DIMIR
        RB,//RAKDOS
        GR,//GRUUL
        GU,//SIMIC
        WB,//ORZHOV
        RU,//IZZET
        GB,//GOLGARI
        RW,//BOROS
        GBW,//ABZAN
        GWU,//BANT
        WRU,//JESKAI
        GRW,//NAYA
        WUB,//ESPER
        GUR,//TEMUR
        GRB,//JUND
        RUB,//GRIXIS
        BGU,//SULTAI
        RWB,//MARDU
        BGUR,//noWhite
        GURW,//noBlack
        URWB,//noGreen
        RWBG,//noBlue
        WBGU,//noRed
        WBGUR,//AllColors
    }

}
