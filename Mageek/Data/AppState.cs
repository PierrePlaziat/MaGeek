using MaGeek.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaGeek.Data
{
    public  class AppState
    {
        public MagicDeck CurrentDeck { get; set; }
        public string SelectedTraduction = "French";
    }

}
