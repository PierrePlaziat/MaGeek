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

        public virtual MagicCard Card { get; set; }

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
            Directory.CreateDirectory(@"./CardsIllus");
            string localFileName = @"./CardsIllus/" + Id+".png";
            if (!File.Exists(localFileName))
            {
                WebClient webClient = new();
                webClient.DownloadFile(ImageUrl, localFileName);
            }
            Uri Url;
            try   { Url = new Uri("file:///" + localFileName, UriKind.Relative); }
            catch { Url = new Uri(ImageUrl,      UriKind.Absolute); }
            BitmapImage img = new(Url);
            return img;
        }

    }
}
