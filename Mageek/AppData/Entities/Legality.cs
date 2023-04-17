using System.ComponentModel.DataAnnotations;
using System.Windows.Media;

namespace MaGeek.AppData.Entities
{

    public class Legality
    {

        [Key]
        public int Id { get; set; }
        public string MultiverseId { get; set; }
        public string Format { get; set; }
        public string IsLegal { get; set; }

        public Brush IsLegalColor { get { return IsLegal == "legal" ? Brushes.IndianRed : Brushes.DarkSeaGreen; } }

    }

}
