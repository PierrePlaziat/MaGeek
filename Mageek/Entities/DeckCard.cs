using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Input;

namespace MaGeek.Entities
{

    public class DeckCard
    {

        [Key, Column(Order = 0)]
        public int DeckId { get; set; }
        [Key, Column(Order = 1)]
        public string CardId { get; set; }
        public virtual Deck Deck { get; set; }
        public virtual CardVariant Card { get; set; }
        public int Quantity { get; set; }
        /// <summary>
        /// 0:normal, 1:commandant , 2:sideDeck
        /// </summary>
        public int RelationType { get; set; }

        [NotMapped]
        public ICommand ChangeIlluCommand { get; set; }

        public DeckCard()
        {
            ChangeIlluCommand = new ChangeCardRelationVariantCommand(this);
        }

    }

}
