using System.ComponentModel.DataAnnotations;

namespace MaGeek.Entities
{
    public class Param
    {
        [Key]
        public string ParamName { get; set; }
        public string ParamValue { get; set; }
    }
}
