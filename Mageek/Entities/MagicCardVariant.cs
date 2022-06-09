using MtgApiManager.Lib.Model;
using System.ComponentModel.DataAnnotations;
using System.Windows.Media.Imaging;

namespace MaGeek.Data.Entities
{
    public class MagicCardVariant
    {
        [Key]
        public string Id { get; set; }
        public string ImageUrl { get; set; }
        public string Rarity { get; set; }
        public string SetName { get; set; }
        //public BitmapImage Image { get; set; }

        public virtual MagicCard card { get; set; }

        public MagicCardVariant(){}

        public MagicCardVariant(ICard selectedCard)
        {
            Id = selectedCard.Id;
            ImageUrl = selectedCard.ImageUrl!=null? selectedCard.ImageUrl.ToString():"";
            Rarity = selectedCard.Rarity;
            SetName = selectedCard.SetName;
        }

    }
}
