using MaGeek.Commands;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Input;

namespace MaGeek.Data.Entities
{

    public class CardDeckRelation
    {

        [Key, Column(Order = 0)]
        public int DeckId { get; set; }
        [Key, Column(Order = 1)]
        public string CardId { get; set; }
        public virtual MagicDeck Deck { get; set; }
        public virtual MagicCardVariant Card { get; set; }
        public int Quantity { get; set; }
        public int RelationType{ get; set; } // ==0 normal, 1==commandant , 2 sideDeck

        [NotMapped]
        public ICommand ChangeIlluCommand { get; set; }

        public CardDeckRelation() {
            ChangeIlluCommand = new ChangeCardRelationVariantCommand(this);
        }

    }

}
