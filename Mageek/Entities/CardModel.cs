using MaGeek.AppBusiness;
using ScryfallApi.Client.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MaGeek.Entities
{
    public class CardModel
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

        public string FavouriteVariant { get; set; } = "";    // dont use in next version
        //public int Got { get; set; }    // dont use in next version

        public virtual List<CardVariant> Variants { get; set; } = new List<CardVariant>();
        public virtual List<CardTraduction> Traductions { get; set; } = new List<CardTraduction>();

        #endregion

        #region Accessors

        public string CardForeignName { get { return MageekCollection.GetTraduction(CardId).Result; } }
        public string MeanPrice
        {
            get
            {
                float total = 0;
                int count = 0;
                foreach (CardVariant variant in Variants)
                {
                    if (!string.IsNullOrEmpty(variant.Value))
                    {
                        count++;
                        total += float.Parse(variant.Value);
                    }
                }
                if (count > 0) return (total / count).ToString("0.##");
                else return "";
            }
        }

        public int Got
        {
            get
            {
                return MageekCollection.GotCard_HaveOne(this).Result;
            }
        }

        #endregion

        #region CTOR

        public CardModel() { }

        public CardModel(string CardId, string Type, string Text,
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
            DevotionB = ExtractDevotion("B", ManaCost);
            DevotionW = ExtractDevotion("W", ManaCost);
            DevotionU = ExtractDevotion("U", ManaCost);
            DevotionG = ExtractDevotion("G", ManaCost);
            DevotionR = ExtractDevotion("R", ManaCost);
        }

        public CardModel(Card scryCard)
        {
            CardId = scryCard.Name;
            Type = scryCard.TypeLine;
            Text = scryCard.OracleText;
            KeyWords = ConcatenateKeywords(scryCard.Keywords);
            Power = scryCard.Power;
            Toughness = scryCard.Toughness;
            ManaCost = scryCard.ManaCost ?? "";
            Cmc = Convert.ToSingle(scryCard.Cmc);
            ColorIdentity = ConcatenateColorIdentity(scryCard.ColorIdentity);
            DevotionB = ExtractDevotion("B", scryCard.ManaCost);
            DevotionW = ExtractDevotion("W", scryCard.ManaCost);
            DevotionU = ExtractDevotion("U", scryCard.ManaCost);
            DevotionG = ExtractDevotion("G", scryCard.ManaCost);
            DevotionR = ExtractDevotion("R", scryCard.ManaCost);
        }

        public static int ExtractDevotion(string color, string ManaCost)
        {
            int devotion = ManaCost != null ? ManaCost.Length - ManaCost.Replace(color, "").Length : 0;
            return devotion;
        }

        public static string ConcatenateKeywords(string[] keywords)
        {
            string k = "";
            bool first = true;
            foreach (string keyword in keywords)
            {
                if (!first) k += ", ";
                k += keyword;
                first = false;
            }
            return k;
        }

        public static string ConcatenateColorIdentity(string[] colorIdentity)
        {
            string colors = "";
            foreach (string color in colorIdentity) colors += color;
            return colors;
        }

        #endregion

    }
}
