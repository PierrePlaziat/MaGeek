#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

using System.ComponentModel.DataAnnotations;

namespace MageekSdk.MtgSqlive.Entities
{
    public class SetBoosterSheets
    {
        [Key]
        public string SetCode { get; set; }
        public bool SheetHasBalanceColors { get; set; }
        public bool SheetIsFoil { get; set; }
        public string SheetName { get; set; }
    }
}
