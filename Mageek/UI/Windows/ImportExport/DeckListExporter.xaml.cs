using MaGeek.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MaGeek.UI.Windows.ImportExport
{
    public partial class DeckListExporter : Window
    {

        public DeckListExporter()
        {
            InitializeComponent();
            ExportBox.Text = ExportList(App.State.SelectedDeck);
        }

        private string ExportList(MagicDeck selectedDeck)
        {
            if (selectedDeck == null) return "No deck selected.";
            string result = "";
            result += selectedDeck.Title +"\n\n";
            foreach(var v in selectedDeck.CardRelations)
            {
                result += v.Quantity + " " + v.Card.Card.CardId+ "\n";
            }
            return result;
        }

    }
}
