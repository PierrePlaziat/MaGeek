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

    public class MagicCardVariant
    {

        [Key]
        public string Id { get; set; }
        public string ImageUrl_Front { get; set; }
        public string ImageUrl_Back { get; set; }

        public string Rarity { get; set; }

        public string Lang { get; set; }

        public string ValueEur { get; set; }
        public string ValueUsd { get; set; }
        public int EdhRecRank { get; set; }

        public string Artist { get; set; }
        public string SetName { get; set; }

        public int IsCustom { get; set; }
        public string CustomName { get; set; }

        public int Got { get; set; }
        public string LastUpdate { get; set; } = "";


        public virtual ICollection<CardDeckRelation> DeckRelations { get; set; }
        public virtual MagicCard Card { get; set; }

        #region CTOR

        public MagicCardVariant() { }
        
        public MagicCardVariant(string Id, string Rarity, string Artist, 
                                string Lang, string SetName, string CardId) 
        {
            this.Id = Id;
            this.Rarity = Rarity;
            this.Artist = Artist;
            this.Lang = Lang;
            this.SetName = SetName;
            IsCustom = 0;
            Got = 0;
            Card = MageekUtils.FindCardById(CardId).Result;
        }

        public MagicCardVariant(Card scryCard)
        {
            Id = scryCard.Id.ToString();
            Rarity = scryCard.Rarity;
            Artist = scryCard.Artist;
            Lang = scryCard.Language;
            SetName = scryCard.SetName;
            IsCustom = 0;
            Got = 0;
            Card = MageekUtils.FindCardById(scryCard.Name).Result;
        }

        #endregion

        #region Methods

        public async Task<BitmapImage> RetrieveImage(bool back = false)
        {
            return await MageekApi.RetrieveImage(this,back);
        }

        public Brush LineColoration
        {
            get
            {
                return string.IsNullOrEmpty(ImageUrl_Front) ? Brushes.Black : Brushes.White;
            }
        }

        public Brush GetPriceColor {
            get
            {
                if (ValueEur == null) return Brushes.Black;
                float p = 0;
                try
                {
                    p = float.Parse(ValueEur);
                }
                catch { }
                if (p >= 10) return Brushes.White;
                else if (p >= 5) return Brushes.Orange;
                else if (p >= 2) return Brushes.Yellow;
                else if (p >= 1) return Brushes.Green;
                else if (p >= 0.2) return Brushes.LightGray;
                else if (p >= 0) return Brushes.DarkGray;
                else return Brushes.Black;
            }
        }

        #endregion

    }

}
