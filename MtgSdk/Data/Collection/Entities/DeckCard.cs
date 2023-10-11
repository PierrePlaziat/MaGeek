#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

using MageekSdk.Data.Mtg.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MageekSdk.Data.Collection.Entities
{

    public class DeckCard
    {

        public Cards Card { get { return MageekService.FindCard_Data(CardUuid).Result; } }


        [Key, Column(Order = 0)]
        public string DeckId { get; set; }
        [Key, Column(Order = 1)]
        public string CardUuid { get; set; }
        public int Quantity { get; set; }
        /// <summary>
        /// 0:normal, 1:commandant , 2:sideDeck
        /// </summary>
        public int RelationType { get; set; }



    }
}
