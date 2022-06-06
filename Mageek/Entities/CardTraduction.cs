using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaGeek.Data.Entities
{

    public class CardTraduction
    {
        [Key]
        public int TraductionId { get; set; }
        public string CardId { get; set; }
        public string Language { get; set; }
        public string TraductedName { get; set; }

        public virtual MagicCard Card{ get; set; }
    }

}
