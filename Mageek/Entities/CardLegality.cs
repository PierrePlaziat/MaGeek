using System.ComponentModel.DataAnnotations;
using System.Windows.Media;

namespace MaGeek.Entities
{

    public class CardLegality
    {

        [Key] public int LegalityId { get; set; }

        public string CardId { get; set; }
        public string Format { get; set; }
        public string IsLegal { get; set; }

        public string LastUpdate { get; set; } = "";

        public Brush IsLegalColor
        {
            get
            {
                if (IsLegal == "legal") return Brushes.DarkSeaGreen;
                if (IsLegal == "restricted") return Brushes.GreenYellow;
                if (IsLegal == "banned") return Brushes.IndianRed;
                return Brushes.CornflowerBlue;
            }
        }

    }

}
