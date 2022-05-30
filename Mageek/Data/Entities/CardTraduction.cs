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
        public int Id { get; set; }
        public string Name_VO { get; set; }
        public string Language { get; set; }
        public string TraductedName { get; set; }
    }

}
