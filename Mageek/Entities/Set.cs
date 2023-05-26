using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace MaGeek.Entities
{
    public class Set
    {
        [Key]
        public string Name { get; set; }
        public string Type { get; set; }
        public string Svg { get; set; }
        public DateOnly Date { get; set; }


        public virtual ObservableCollection<CardVariant> SetCards { get; set; }


    }
}
