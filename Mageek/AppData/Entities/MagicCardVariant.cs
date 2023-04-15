using MaGeek.AppBusiness;
using MtgApiManager.Lib.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
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
        public int IsCustom { get; set; }
        public string CustomName { get; set; }
        public int Got { get; set; }

        public virtual ICollection<CardDeckRelation> DeckRelations { get; set; }
        public virtual MagicCard Card { get; set; }

        #region CTOR

        public MagicCardVariant() { }

        public MagicCardVariant(ICard selectedCard)
        {
            Id = selectedCard.Id;
            ImageUrl = selectedCard.ImageUrl != null ? selectedCard.ImageUrl.ToString() : "";
            Rarity = selectedCard.Rarity;
            SetName = selectedCard.SetName;
            IsCustom = 0;
            Got = 0;
            MultiverseId = selectedCard.MultiverseId;
            Card = App.Biz.Utils.FindCardById(selectedCard.Name).Result;
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

}
