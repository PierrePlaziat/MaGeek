using MaGeek.AppBusiness;
using ScryfallApi.Client.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MaGeek.AppData.Entities
{

    public class CardVariant
    {

        [Key]
        public string Id { get; set; }
        public string Rarity { get; set; }
        public string SetName { get; set; }
        public string ImageUrl_Front { get; set; }
        public string ImageUrl_Back { get; set; }
        public string ValueEur { get; set; }
        public string ValueUsd { get; set; }
        public int EdhRecRank { get; set; }
        public int IsCustom { get; set; }
        public string CustomName { get; set; }
        public int Got { get; set; }
        public string LastUpdate { get; set; } = "";


        public virtual ICollection<DeckCard> DeckRelations { get; set; }
        public virtual CardModel Card { get; set; }

        #region CTOR

        public CardVariant() { }
        
        public CardVariant(string Id, string Rarity, string Artist, 
                                string Lang, string SetName, CardModel Card) 
        {
            this.Id = Id;
            this.Rarity = Rarity;
            this.SetName = SetName;
            this.Card = Card;
            IsCustom = 0;
            Got = 0;
        }

        public CardVariant(Card scryCard)
        {
            Id = scryCard.Id.ToString();
            Rarity = scryCard.Rarity;
            SetName = scryCard.SetName;
            IsCustom = 0;
            Got = 0;
            Card = MageekCollection.QuickFindCardById(scryCard.Name).Result;
        }

        #endregion

        #region Methods

        public async Task<BitmapImage> RetrieveImage(bool back = false)
        {
            return await MageekApi.RetrieveImage(this,back);
        }

        public Brush GetPriceColor {
            get
            {
                if (ValueEur == null) return Brushes.Black;
                float p = 0;
                if (ValueEur != null) p = float.Parse(ValueEur);
                if (p >= 10) return Brushes.Yellow;
                else if (p >= 5) return Brushes.Orange;
                else if (p >= 2) return Brushes.Red;
                else if (p >= 1) return Brushes.White;
                else if (p >= 0.5) return Brushes.LightGray;
                else if (p >= 0) return Brushes.DarkGray;
                else return Brushes.Black;
            }
        }
        
        public Brush GetRarityColor {
            get
            {
                if (string.IsNullOrEmpty(Rarity)) return Brushes.Black;
                else if (Rarity == "common") return Brushes.LightGray;
                else if (Rarity == "uncommon") return Brushes.CadetBlue;
                else if (Rarity == "rare") return Brushes.Gold;
                else if (Rarity == "mythic") return Brushes.OrangeRed;
                else return Brushes.Green;
            }
        }

        #endregion

    }

}
