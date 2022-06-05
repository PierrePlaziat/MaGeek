using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaGeek.Entities
{

    internal class CardDeckRelation
    {
        public int DeckId { get; set; }
        public string CardName { get; set; }
        public int Quantity{ get; set; }
    }

}
