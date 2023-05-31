using System;
using System.ComponentModel.DataAnnotations;

namespace MaGeek.Entities
{
    public class MtgSet
    {
        [Key]
        public string Name { get; set; }
        public string Type { get; set; }
        public string Block { get; set; }
        public int BaseSetSize { get; set; }
        public int TotalSetSize { get; set; }
        public DateOnly ReleaseDate { get; set; }
        public string Svg { get; set; }

    }
}
