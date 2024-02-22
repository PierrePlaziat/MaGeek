using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MageekCore.Data.Collection.Entities
{

    public class DeckCard
    {

        [Key, Column(Order = 0)]
        public string DeckId { get; set; }
        [Key, Column(Order = 1)]
        public string CardUuid { get; set; }
        public int Quantity { get; set; }
        /// <summary>
        /// 0:normal, 1:commandant , 2:sideDeck
        /// </summary>
        public int RelationType { get; set; }

        public string CardType { get; set; } // TODO

    }

}
