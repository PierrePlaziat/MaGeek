using MaGeek.AppFramework;
using MtgApiManager.Lib.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
        public float? Cmc { get; set; }
        public string Text { get; set; }
        public string Power { get; set; }
        public string Toughness { get; set; }
        public virtual List<MagicCardVariant> Variants { get; set; } = new List<MagicCardVariant>();
        public virtual List<CardTraduction> Traductions { get; set; } = new List<CardTraduction>();
        public string FavouriteVariant { get; set; } = "";

        #endregion

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
        }

        public void AddVariant(ICard iCard)
        {
            MagicCardVariant variant = Variants.Where(x => x.Id == iCard.Id).FirstOrDefault();
            if (variant != null) return;
            variant = new MagicCardVariant(iCard);
            Variants.Add(variant);
            AddNames(iCard.ForeignNames);
            // 
            if (string.IsNullOrEmpty(FavouriteVariant) && !string.IsNullOrEmpty(variant.ImageUrl)) FavouriteVariant = variant.Id;
        }

        private void AddNames(List<IForeignName> foreignNames)
        {
            if (foreignNames == null) return;
            if (Traductions == null) Traductions = new List<CardTraduction>();
            foreach (IForeignName foreignName in foreignNames)
            {
                CardTraduction trad = Traductions.Where(x => x.Language == foreignName.Language).FirstOrDefault();
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
            foreach (var variant in Variants)
            {
                if (!string.IsNullOrEmpty(variant.ImageUrl))
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
                var a = Traductions.Where(x => x.Language == App.Config.Settings[Setting.ForeignLangugage]).FirstOrDefault();
                return a != null ? a.TraductedName : CardId;
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
