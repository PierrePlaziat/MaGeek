using System.ComponentModel.DataAnnotations;

namespace MaGeek
{
    public class User_GotCard
    {
        [Key]
        public string CardVariantId { get; set; }
        public string CardModelId { get; set; }
        public int got { get; set; }

    }
}