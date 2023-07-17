#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

using System.ComponentModel.DataAnnotations;

namespace MageekSdk.MtgSqlive.Entities
{
    public class SetBoosterContents
    {
        [Key]
        public int BoosterIndex { get; set; }
        public string BoosterName { get; set; }
        public int BoosterWeight { get; set; }
        public string SetCode { get; set; }
    }
}
