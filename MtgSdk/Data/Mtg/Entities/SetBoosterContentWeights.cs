﻿#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

using System.ComponentModel.DataAnnotations;

namespace MageekService.Data.Mtg.Entities
{
    public class SetBoosterContentWeights
    {
        [Key]
        public int BoosterIndex { get; set; }
        public string BoosterName { get; set; }
        public string SetCode { get; set; }
        public string SheetName { get; set; }
        public int SheetPicks { get; set; }
    }
}
