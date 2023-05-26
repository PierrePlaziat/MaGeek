using MaGeek.Entities;
using System.Windows;

namespace MaGeek.UI.Windows.ImportExport
{
    public partial class DeckListExporter : Window
    {

        public DeckListExporter(string missList)
        {
            if (missList != null) ExportBox.Text = missList;
        }

        public DeckListExporter(Deck selectedDeck, bool setAware = false)
        {
            InitializeComponent();
            if (selectedDeck != null) ExportBox.Text = ExportList(App.State.SelectedDeck);
        }

        private string ExportList(Deck selectedDeck)
        {
            if (selectedDeck == null) return "No deck selected.";
            string result = "";
            result += selectedDeck.Title +"\n\n";
            foreach(var v in selectedDeck.DeckCards)
            {
                result += v.Quantity + " " + v.Card.Card.CardId+ "\n";
            }
            return result;
        }

    }
}
