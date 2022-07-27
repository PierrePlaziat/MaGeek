using MtgApiManager.Lib.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Threading.Tasks;
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
        public int IsCustom { get; set; }
        public string CustomName { get; set; }

        public virtual ICollection<CardDeckRelation> DeckRelations { get; set; }
        public virtual MagicCard Card { get; set; }

        #region CTOR

        public MagicCardVariant(){ }

        public MagicCardVariant(ICard selectedCard)
        {
            Id = selectedCard.Id;
            ImageUrl = selectedCard.ImageUrl != null ? selectedCard.ImageUrl.ToString() : "";
            Rarity = selectedCard.Rarity;
            SetName = selectedCard.SetName;
            IsCustom = 0;
        }

        #endregion

        public async Task<BitmapImage> RetrieveImage()
        {
            var taskCompletion = new TaskCompletionSource<BitmapImage>();
            BitmapImage img = null;
            string localFileName = "";
            if(IsCustom==0)
            {
                localFileName = @"./CardsIllus/" + Id + ".png";
                if (!File.Exists(localFileName))
                {
                    await Task.Run(async () => { 
                        await new WebClient().DownloadFileTaskAsync(ImageUrl, localFileName); 
                    });
                }
            }
            else
            {
                localFileName = @"./CardsIllus/Custom/" + ImageUrl;
            }
            var path = Path.GetFullPath(localFileName);
            Uri imgUri = new Uri("file://" + path, UriKind.Absolute);
            img = new BitmapImage(imgUri);
            taskCompletion.SetResult(img);
            return img;
        }

    }
}
