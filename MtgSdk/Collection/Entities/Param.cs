using System.ComponentModel.DataAnnotations;

namespace MageekSdk.Collection.Entities
{
    public class Param
    {
        [Key]
        public string ParamName { get; set; }
        public string ParamValue { get; set; }
    }
}
