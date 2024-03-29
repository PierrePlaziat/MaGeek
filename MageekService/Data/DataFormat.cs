﻿using MageekCore.Data.Collection.Entities;
using MageekCore.Data.Mtg.Entities;

namespace MageekCore.Data
{

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

        public float? GetPrice //TODO multi monaie & colors
        {
            get
            {
                return 0;
                //if (Price == null) return null;
                //if (Price.GetLastPriceEur == null) return null;
                //return Price.GetLastPriceEur;
            }
        }
    }

    public class SearchedCards
    {

        public SearchedCards(Cards card, int collected, CardForeignData translation)
        {
            Card = card;
            Collected = collected;
            if(translation!=null) Translation = translation.Name;
        }

        public Cards Card { get; }
        public string Translation { get; } = string.Empty;
        public int Collected { get; }

    }

    public class TxtImportResult
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
        public required List<Tuple<string,int>> CommanderCardUuids { get; set; }
        public required List<Tuple<string, int>> MainCardUuids { get; set; }
        public required List<Tuple<string, int>> SideCardUuids { get; set; }

        public  int NbCards {
            get 
            {
                int i = 0;
                foreach(var card in CommanderCardUuids)
                {
                    i += card.Item2;
                }
                foreach(var card in MainCardUuids)
                {
                    i += card.Item2;
                }
                return i;
            }
        }
        public  int NbCardsSide
        {
            get
            {
                int i = 0;
                foreach (var card in SideCardUuids)
                {
                    i += card.Item2;
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
        MtgUpToDate,
        MtgOutdated,
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
