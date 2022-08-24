using System.Windows;
using System.Windows.Controls;

namespace MaGeek.UI.CustomControls
{

    public partial class TopMenu : UserControl
    {

        public TopMenu()
        {
            InitializeComponent();
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            var window = new Importer();
            window.Width = 260;
            window.Show();
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Params_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CardSearcher_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeckList_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeckContent_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveCurrentLayout(object sender, RoutedEventArgs e)
        {

        }

        private void ResetDefaultLayout(object sender, RoutedEventArgs e)
        {

        }
    }

}
