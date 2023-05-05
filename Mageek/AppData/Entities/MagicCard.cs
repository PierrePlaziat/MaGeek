using MaGeek.AppBusiness;
using ScryfallApi.Client.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MaGeek.AppData.Entities
{
    public class MagicCard
    {

        #region Entity

        [Key]
        public string CardId { get; set; }
        public string Type { get; set; }

        public string ManaCost { get; set; }
        public float Cmc { get; set; }
        public string ColorIdentity { get; set; }
        public int DevotionB { get; set; }
        public int DevotionW { get; set; }
        public int DevotionU { get; set; }
        public int DevotionG { get; set; }
        public int DevotionR { get; set; }

        public string Text { get; set; }
        public string KeyWords { get; set; }

        public string Power { get; set; }
        public string Toughness { get; set; }

        public string FavouriteVariant { get; set; } = "";
        public virtual List<MagicCardVariant> Variants { get; set; } = new List<MagicCardVariant>();

        public virtual List<CardTraduction> Traductions { get; set; } = new List<CardTraduction>();
        
        #endregion

        #region CTOR

        public MagicCard() { }
        
        public MagicCard(string CardId, string Type, string Text, 
                         string KeyWords, string Power, string Toughness,
                         string ManaCost, float Cmc, string ColorIdentity) 
        {
            this.CardId = CardId;
            this.Type = Type;
            this.Text = Text;
            this.KeyWords = KeyWords;
            this.Power = Power;
            this.Toughness = Toughness;
            this.ManaCost = ManaCost;
            this.Cmc = Cmc;
            this.ColorIdentity = ColorIdentity;
            DevotionB = ParseDevotion("B");
            DevotionW = ParseDevotion("W");
            DevotionU = ParseDevotion("U");
            DevotionG = ParseDevotion("G");
            DevotionR = ParseDevotion("R");
        }

        public MagicCard(Card scryCard)
        {
            CardId = scryCard.Name;
            Type = scryCard.TypeLine;
            Text = scryCard.OracleText;
            SetKeyWords(scryCard.Keywords);
            Power = scryCard.Power;
            Toughness = scryCard.Toughness;
            ManaCost = scryCard.ManaCost ?? "";
            Cmc = Convert.ToSingle(scryCard.Cmc);
            SetColorIdentity(scryCard.ColorIdentity);
            DevotionB = ParseDevotion("B");
            DevotionW = ParseDevotion("W");
            DevotionU = ParseDevotion("U");
            DevotionG = ParseDevotion("G");
            DevotionR = ParseDevotion("R");
        }

        private void SetKeyWords(string[] keywords)
        {
            string k = "";
            bool first = true;
            foreach (string keyword in keywords)
            {
                if (!first) k += ", ";
                k+= keyword; 
                first = false;
            }
            KeyWords = k;
        }

        private int ParseDevotion(string color)
        {
            int devotion = ManaCost != null ? ManaCost.Length - ManaCost.Replace(color, "").Length : 0;
            return devotion;
        }

        private void SetColorIdentity(string[] colorIdentity)
        {
            ColorIdentity = "";
            foreach (string color in colorIdentity) ColorIdentity += color;
        }

        public async Task<BitmapImage> RetrieveImage(int selectedVariant = -1)
        {
            if (selectedVariant != -1)
            {
                if (!string.IsNullOrEmpty(Variants[selectedVariant].ImageUrl_Front))
                {
                    return await Variants[selectedVariant].RetrieveImage();
                }
            }
            foreach (var variant in Variants)
            {
                if (!string.IsNullOrEmpty(variant.ImageUrl_Front))
                {
                    return await variant.RetrieveImage();
                }
            }
            return null;
        }

        #endregion

        #region Accessors

        public string CardForeignName
        {
            get
            {
                return MageekUtils.GetTraduction(CardId).Result;
            }
        }

        public int Got
        {
            get
            {
                if (Variants == null) return 0;
                int q = 0;
                foreach (var v in Variants) q += v.Got;
                return q;
            }
        }

        #endregion

    }
}
