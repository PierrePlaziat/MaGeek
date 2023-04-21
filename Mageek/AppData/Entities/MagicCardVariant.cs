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
        public string SetName { get; set; } = "";
        public int IsCustom { get; set; }
        public string CustomName { get; set; }
        public int Got { get; set; }

        public string LastUpdate { get; set; } = "";


        public virtual ICollection<CardDeckRelation> DeckRelations { get; set; }
        public virtual MagicCard Card { get; set; }

        #region CTOR

        public MagicCardVariant() { }

        public MagicCardVariant(Card selectedCard)
        {
            Id = selectedCard.Id.ToString();
            ImageUrl = selectedCard.ImageUris != null ? selectedCard.ImageUris.Values.LastOrDefault().ToString() : "";
            Rarity = selectedCard.Rarity;
            SetName = selectedCard.SetName != null ? selectedCard.SetName : "???";
            IsCustom = 0;
            Got = 0;
            MultiverseId = selectedCard.MultiverseIds != null ? selectedCard.MultiverseIds.FirstOrDefault().ToString() : "";
            Card = MageekUtils.FindCardById(selectedCard.Name).Result;
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

        #endregion

    }

    internal class CardEphemeralInfos
    {
        public CardValue values = new CardValue();
        public Dictionary<string, string> legals = new Dictionary<string, string>();
        public List<MagicCard> relateds = new List<MagicCard>();
    }

}
