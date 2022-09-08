using MaGeek.UI.Windows.Importers;
using MaGeek.UI.Windows.ImportExport;
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

        #region Import Export

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            var window = new TxtImporter();
            window.Width = 260;
            window.Show();
        }

        private void OpenWindow_SetImporter(object sender, RoutedEventArgs e)
        {
            var window = new SetImporter();
            window.Show();
        }

        private void OpenWindow_PrecoImporter(object sender, RoutedEventArgs e)
        {
            var window = new PrecoImporter();
            window.Show();
        }

        private void OpenWindow_DeckListExporter(object sender, RoutedEventArgs e)
        {
            var window = new DeckListExporter();
            window.Show();
        }

        private void OpenWindow_ProxyPrint(object sender, RoutedEventArgs e)
        {
            var window = new ProxyPrint();
            window.Show();
        }



        #endregion

        #region Tools

        private void Params_Click(object sender, RoutedEventArgs e)
        {
            var window = new Options();
            window.Show();
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

        #endregion

        #region Layout

        private void SaveCurrentLayout(object sender, RoutedEventArgs e)
        {

        }

        private void ResetDefaultLayout(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        private void AboutClicked(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/PierrePlaziat/MaGeek");
        }
    }

}
