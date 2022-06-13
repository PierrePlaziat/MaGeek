using MaGeek.Entities;
using MtgApiManager.Lib.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MaGeek.Data.Entities
{
    public class MagicCard
    {

        [Key]
        public string CardId { get; set; }

        public string Type { get; set; }
        public string ManaCost { get; set; }
        public float? Cmc { get; set; }
        public string Text { get; set; }
        public string Power { get; set; }
        public string Toughness { get; set; }
        public int CollectedQuantity { get; set; }
        public virtual List<MagicCardVariant> Variants { get; set; } = new List<MagicCardVariant>();
        public virtual List<CardTraduction> Traductions { get; set; } = new List<CardTraduction>();

        public virtual ICollection<CardDeckRelation> DeckRelations { get; set; }

        public string CardForeignName {
            get
            {
                try
                {
                    return Traductions.Where( x=> x.Language.ToLower() == App.state.GetForeignLanguage().ToLower() ).FirstOrDefault().TraductedName;
                }
                catch 
                {
                    return "(?)"+CardId;
                }
            }
        }

        #region CTOR

        public MagicCard() { }

        public MagicCard(ICard selectedCard)
        {
            CardId = selectedCard.Name;
            Type = selectedCard.Type;
            ManaCost = selectedCard.ManaCost ?? "";
            Cmc = selectedCard.Cmc;
            Text = selectedCard.Text;
            Power = selectedCard.Power;
            Toughness = selectedCard.Toughness;
            AddNames(selectedCard.ForeignNames);
            //AddLegalities(selectedCard.Legalities);
            CollectedQuantity = 0;
        }

        public void AddVariant(ICard iCard)
        {
            MagicCardVariant variant = Variants.Where(x=>x.Id == iCard.Id).FirstOrDefault();
            if (variant != null) return;
            variant = new MagicCardVariant(iCard);
            Variants.Add(variant);
            AddNames(iCard.ForeignNames);
        }

        private void AddNames(List<IForeignName> foreignNames)
        {
            if (foreignNames == null) return;   
            if (Traductions==null) Traductions = new List<CardTraduction>();
            foreach (IForeignName foreignName in foreignNames)
            {
                CardTraduction trad = Traductions.Where(x=>x.Language == foreignName.Language).FirstOrDefault();
                if (trad != null) return;
                Traductions.Add(
                    new CardTraduction()
                    {
                        CardId = CardId,
                        Language = foreignName.Language,
                        TraductedName = foreignName.Name
                    }
                );
            }
        }

        public async Task<BitmapImage> RetrieveImage(int selectedVariant = -1)
        {
            if (selectedVariant != -1)
            {
                if (!string.IsNullOrEmpty(Variants[selectedVariant].ImageUrl))
                {
                    return await Variants[selectedVariant].RetrieveImage();
                }
            }
            foreach(var variant in Variants)
            {
                if(!string.IsNullOrEmpty(variant.ImageUrl))
                {
                    return await variant.RetrieveImage();
                }
            }
            return null;
        }

        /*private void AddLegalities(List<ILegality> legalities)
        {
            if (Legalities==null) 
                Legalities = new Dictionary<string, string>();
            foreach (ILegality legality in legalities)
            {
                Legalities.Add(legality.Format, legality.LegalityName);
            }
        }*/

        #endregion

        #region Accessors

        public int DevotionB { get { return ManaCost != null ? ManaCost.Length - ManaCost.Replace("B", "").Length : 0;  } }
        public int DevotionW { get { return ManaCost != null ? ManaCost.Length - ManaCost.Replace("W", "").Length : 0; } }
        public int DevotionU { get { return ManaCost != null ? ManaCost.Length - ManaCost.Replace("U", "").Length : 0; } }
        public int DevotionG { get { return ManaCost != null ? ManaCost.Length - ManaCost.Replace("G", "").Length : 0; } }
        public int DevotionR { get { return ManaCost != null ? ManaCost.Length - ManaCost.Replace("R", "").Length : 0; } }

        #endregion

    }
}
