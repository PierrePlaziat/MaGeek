using MaGeek.Entities;
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

        public virtual ICollection<CardDeckRelation> DeckRelations { get; set; }

        public virtual MagicCard Card { get; set; }

        public MagicCardVariant(){}

        public MagicCardVariant(ICard selectedCard)
        {
            Id = selectedCard.Id;
            ImageUrl = selectedCard.ImageUrl!=null? selectedCard.ImageUrl.ToString():"";
            Rarity = selectedCard.Rarity;
            SetName = selectedCard.SetName;
        }

        public async Task<BitmapImage> RetrieveImage()
        {
            BitmapImage img = null;
            var taskCompletion = new TaskCompletionSource<BitmapImage>();
            string localFileName = @"./CardsIllus/" + Id + ".png";
            if (!File.Exists(localFileName)) new WebClient().DownloadFile(ImageUrl, localFileName);
            var path = Path.GetFullPath(localFileName);
            Uri imgUri = new Uri("file://" + path, UriKind.Absolute); 
            img = new BitmapImage(imgUri);
            taskCompletion.SetResult(img);
            return img;
        }

        public async Task<BitmapImage> RetrieveImageOld()
        {
            var taskCompletion = new TaskCompletionSource<BitmapImage>();
            Uri imgUri = new Uri(ImageUrl, UriKind.Absolute);
            BitmapImage img = new BitmapImage(imgUri);
            taskCompletion.SetResult(img);
            return img;
        }

    }
}
