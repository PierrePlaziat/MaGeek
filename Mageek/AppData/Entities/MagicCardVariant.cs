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
        public string ImageUrl { get; set; }
        public string Rarity { get; set; }
        public string SetName { get; set; }

        public string Lang { get; set; }
        public string TraductedTitle { get; set; }
        public string TraductedText { get; set; }
        public string Traductedype { get; set; }

        public string ValueEur { get; set; }
        public string ValueUsd { get; set; }
        public int EdhRecRank { get; set; }

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
            ImageUrl = scryCard.ImageUris != null ? scryCard.ImageUris.Values.LastOrDefault().ToString() : "";
            Rarity = scryCard.Rarity;
            SetName = scryCard.SetName != null ? scryCard.SetName : "";


            Lang = scryCard.Language;
            TraductedTitle = scryCard.PrintedName;
            TraductedText = scryCard.PrintedText;
            Traductedype = scryCard.PrintedTypeLine;

            IsCustom = 0;
            Got = 0;
            Card = MageekUtils.FindCardById(scryCard.Name).Result;
        }

        #endregion

        #region Methods

        public async Task<BitmapImage> RetrieveImage()
        {
            return await MageekUtils.RetrieveImage(this);
        }

        public Brush LineColoration
        {
            get
            {
                return string.IsNullOrEmpty(ImageUrl) ? Brushes.Black : Brushes.White;
            }
        }
        public Brush GetPriceColor {
            get
            {
                float p = float.Parse(ValueEur);
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
