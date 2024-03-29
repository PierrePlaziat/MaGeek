﻿#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

using System.ComponentModel.DataAnnotations;

namespace MageekCore.Data.Mtg.Entities
{
    public class CardPurchaseUrls
    {
        public string CardKingdom { get; set; }
        public string CardKingdomEtched { get; set; }
        public string CardKingdomFoil { get; set; }
        public string Cardmarket { get; set; }
        public string Tcgplayer { get; set; }
        public string TcgplayerEtched { get; set; }
        [Key]
        public string Uuid { get; set; }
    }
}
