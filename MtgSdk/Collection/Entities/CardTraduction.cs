using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MageekSdk.Collection.Entities
{

    public class CardTraduction
    {
        [Key, Column(Order = 0)]
        public string CardUuid { get; set; }
        [Key, Column(Order = 1)]
        public string Language { get; set; }
        public string Traduction { get; set; }
        public string NormalizedTraduction { get; set; }
    }
}
