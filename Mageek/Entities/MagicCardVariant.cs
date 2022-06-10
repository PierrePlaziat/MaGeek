using MtgApiManager.Lib.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
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

        public virtual MagicCard card { get; set; }

        public MagicCardVariant(){}

        public MagicCardVariant(ICard selectedCard)
        {
            Id = selectedCard.Id;
            ImageUrl = selectedCard.ImageUrl!=null? selectedCard.ImageUrl.ToString():"";
            Rarity = selectedCard.Rarity;
            SetName = selectedCard.SetName;
        }

        public BitmapImage RetrieveImage()
        {
            BitmapImage img = null;
            Uri Url = null;
            string localFileName = @"./CardsIllus/" + Id+".png";
            if (!File.Exists(localFileName))
            {
                WebClient webClient = new WebClient();
                webClient.DownloadFile(ImageUrl, localFileName);
            }
            try   { Url = new Uri("file:///" + localFileName, UriKind.Relative); }
            catch { Url = new Uri(ImageUrl,      UriKind.Absolute); }
            img = new BitmapImage(Url);
            return img;
        }

    }
}
