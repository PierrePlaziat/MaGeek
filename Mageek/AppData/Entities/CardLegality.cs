using System.ComponentModel.DataAnnotations;
using System.Windows.Media;

namespace MaGeek.AppData.Entities
{

    public class CardLegality
    {

        [Key]
        public int LegalityId { get; set; }
        public string CardId { get; set; }
        public string Format { get; set; }
        public string IsLegal { get; set; }
        public string LastUpdate { get; set; } = "";

        public Brush IsLegalColor { get { return IsLegal == "legal" ? Brushes.DarkSeaGreen : Brushes.IndianRed; } }

    }

}
