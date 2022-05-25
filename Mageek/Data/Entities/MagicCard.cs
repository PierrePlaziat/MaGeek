using MtgApiManager.Lib.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MaGeek.Data.Entities
{
    public class MagicCard
    {

        [Key]
        public string Name_VO { get; set; }
        public string Name_VF { get; set; }
        public string Type { get; set; }
        public string ManaCost { get; set; }
        public float? Cmc { get; set; }
        public string Text { get; set; }
        public string Power { get; set; }
        public string Toughness { get; set; }
        public int CollectedQuantity { get; set; }

        public virtual List<MagicCardVariant> variants { get; set; } = new List<MagicCardVariant>();
        public virtual ICollection<MagicDeck> Decks { get; set; }

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
            CollectedQuantity = 0;
            if (selectedCard.ForeignNames!=null && selectedCard.ForeignNames.Where(x=>x.Language=="French").Any())
            {
                Name_VF = selectedCard.ForeignNames.Where(x => x.Language == "French").FirstOrDefault().Name;
            }
        }

        public int DevotionB { get { return ManaCost != null ? ManaCost.Length - ManaCost.Replace("B", "").Length : 0;  } }
        public int DevotionW { get { return ManaCost != null ? ManaCost.Length - ManaCost.Replace("W", "").Length : 0; } }
        public int DevotionU { get { return ManaCost != null ? ManaCost.Length - ManaCost.Replace("U", "").Length : 0; } }
        public int DevotionG { get { return ManaCost != null ? ManaCost.Length - ManaCost.Replace("G", "").Length : 0; } }
        public int DevotionR { get { return ManaCost != null ? ManaCost.Length - ManaCost.Replace("R", "").Length : 0; } }

    }
}
