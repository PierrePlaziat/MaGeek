using MaGeek.AppData.Entities;
using System.Windows;

namespace MaGeek.UI.Windows.ImportExport
{
    public partial class DeckListExporter : Window
    {

        public DeckListExporter(string preFill = null)
        {
            InitializeComponent();
            if (preFill==null) ExportBox.Text = ExportList(App.State.SelectedDeck);
            else ExportBox.Text = preFill;
        }

        private string ExportList(Deck selectedDeck)
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
