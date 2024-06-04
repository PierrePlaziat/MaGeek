using MageekCore.Data.Collection.Entities;
using MageekCore.Data.Mtg.Entities;
using MageekCore.Data.MtgFetched.Entities;

namespace MageekCore.Data
{

    public class CardVariant
    {

        public Cards Card { get; set; } //TODO demeler
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

    }

    public class SearchedCards
    {
        public SearchedCards(string cardUuid, string translation, int collected)
        {
            CardUuid = cardUuid;
            Translation = translation;
            Collected = collected;
        }

        public string CardUuid { get; set; }
        public Cards Card { get; set; }
        public string Translation { get; set; } = string.Empty;
        public int Collected { get; set; }

    }

    public class CardList
    {
        public string Status { get; set; }
        public string Detail { get; set; }
        public List<DeckCard> Cards { get; set; }
    }

    public class Preco
    {
        public required string Code { get; set; }
        public required string Title { get; set; }
        public required string ReleaseDate { get; set; }
        public required string Kind { get; set; }
        public List<DeckCard> Cards { get; set; }

        public  int NbCards {
            get 
            {
                int i = 0;
                foreach(var card in Cards.Where(x=> x.RelationType == 0 || x.RelationType == 1))
                {
                    i += card.Quantity;
                }
                return i;
            }
        }
        public  int NbCardsSide
        {
            get
            {
                int i = 0;
                foreach (var card in Cards.Where(x => x.RelationType == 2))
                {
                    i += card.Quantity;
                }
                return i;
            }
        }
    }

    public enum MageekConnectReturn
    {
        Success,
        Failure,
        NotImplementedForServer
    }
    
    public enum MageekInitReturn
    {
        Error,
        UpToDate,
        Outdated,
        NotImplementedForClient
    }

    public enum MageekUpdateReturn
    {
        ErrorDownloading,
        ErrorFetching,
        ErrorFetchingPrecos,
        Success,
        ErrorFetchingRelations,
        ErrorFetchingPrices,
        NotImplementedForClient
    }


    /// <summary>
    /// Relations beetween cards incliding tokens, melds and combos
    /// </summary>
    public class CardRelation
    {
        public CardRelationRole Role { get; set; }
        public string CardUuid { get; set; }
        public string TokenUuid { get; set; }
        public Cards Card { get; set; } //demeler
        public Tokens Token { get; set; } //demeler
    }

    /// <summary>
    /// Scryfall available roles
    /// </summary>
    public enum CardRelationRole
    {
        token, meld_part, meld_result, combo_piece
    }

    /// <summary>
    /// Scryfall available images formats
    /// </summary>
    public enum CardImageFormat
    {
        small = 0, 
        large = 1,
        medium = 2,
        png = 3, 
        art_crop = 4,
        border_crop = 5
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
