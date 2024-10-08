﻿#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

using System.ComponentModel.DataAnnotations;

namespace MageekCore.Data.MtgFetched.Entities
{

    public class ArchetypeCard
    {
        [Key]
        public string CardUuid { get; set; }
        public string ArchetypeId { get; set; }
    }

}
