﻿using MageekService.Data.Mtg.Entities;

namespace MageekService
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