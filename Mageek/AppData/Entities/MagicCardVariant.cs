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
        public string MultiverseId { get; set; }
        public string ImageUrl_Front { get; set; }
        public string ImageUrl_Back { get; set; }

        public string Rarity { get; set; }

        public string Lang { get; set; }
        public string TraductedTitle { get; set; }
        public string TraductedText { get; set; }
        public string TraductedType { get; set; }

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

        public MagicCardVariant(Card scryCard)
        {
            Id = scryCard.Id.ToString();
            MultiverseId = scryCard.MultiverseIds != null ? scryCard.MultiverseIds.FirstOrDefault().ToString() : "";            
            Rarity = scryCard.Rarity;
            Artist = scryCard.Artist;
            Lang = scryCard.Language;
            TraductedTitle = scryCard.PrintedName;
            TraductedText = scryCard.PrintedText;
            TraductedType = scryCard.PrintedTypeLine;
            SetName = scryCard.SetName;
            if (scryCard.ImageUris!=null)
            {
                ImageUrl_Front = scryCard.ImageUris.Values.LastOrDefault().ToString();
                ImageUrl_Back = "";
            }
            else if(scryCard.CardFaces != null)
            {
                ImageUrl_Front = scryCard.CardFaces[0].ImageUris.Values.LastOrDefault().ToString();
                ImageUrl_Back = scryCard.CardFaces[1].ImageUris.Values.LastOrDefault().ToString();
            }
            else{
                ImageUrl_Front = "";
                ImageUrl_Back = "";
            }

            IsCustom = 0;
            Got = 0;
            Card = MageekUtils.FindCardById(scryCard.Name).Result;
        }

        #endregion

        #region Methods

        public async Task<BitmapImage> RetrieveImage(bool back = false)
        {
            return await MageekUtils.RetrieveImage(this,back);
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

    internal class CardEphemeralInfos
    {
        public CardValue values = new CardValue();
        public Dictionary<string, string> legals = new Dictionary<string, string>();
        public List<MagicCard> relateds = new List<MagicCard>();
    }

}
