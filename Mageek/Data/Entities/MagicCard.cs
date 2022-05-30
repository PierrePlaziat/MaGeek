using MtgApiManager.Lib.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MaGeek.Data.Entities
{
    public class MagicCard
    {

        [Key]
        public string Name_VO { get; set; }
        public string Type { get; set; }
        public string ManaCost { get; set; }
        public float? Cmc { get; set; }
        public string Text { get; set; }
        public string Power { get; set; }
        public string Toughness { get; set; }
        public int CollectedQuantity { get; set; }

        public virtual List<MagicCardVariant> variants { get; set; } = new List<MagicCardVariant>();
        public virtual List<CardTraduction> Traductions { get; set; }
        public virtual ICollection<MagicDeck> Decks { get; set; }

        #region CTOR

        public MagicCard() { }

        public MagicCard(ICard selectedCard)
        {
            Name_VO = selectedCard.Name;
            Type = selectedCard.Type;
            ManaCost = selectedCard.ManaCost;
            Cmc = selectedCard.Cmc;
            Text = selectedCard.Text;
            Power = selectedCard.Power;
            Toughness = selectedCard.Toughness;
            AddNames(selectedCard.ForeignNames);
            //AddLegalities(selectedCard.Legalities);
            //selectedCard.Names; // TODO : manage doublesided cards
            CollectedQuantity = 0;
        }

        public void AddVariant(ICard iCard)
        {
            MagicCardVariant variant = variants.Where(x=>x.Id == iCard.Id).FirstOrDefault();
            if (variant != null) return;
            variant = new MagicCardVariant(iCard);
            variants.Add(variant);
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
                        Name_VO = Name_VO,
                        Language = foreignName.Language,
                        TraductedName = foreignName.Name
                    }
                );
            }
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
