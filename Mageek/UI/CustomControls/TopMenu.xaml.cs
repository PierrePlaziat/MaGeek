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

        private void OpenWindow_TxtImporter(object sender, RoutedEventArgs e)
        {
            var window = new TxtImporter();
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

        #region Database

        private void BackupDb(object sender, RoutedEventArgs e)
        {
            App.Database.BackupDb();
        }

        private void RestoreDb(object sender, RoutedEventArgs e)
        {
            App.Database.RestoreDb();
        }

        private void EraseDb(object sender, RoutedEventArgs e)
        {
            App.Database.EraseDb();
        }

        #endregion

        #region Language

        private void ChangeLanguage(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            App.State.SetForeignLanguage(item.Header.ToString());
        }

        #endregion

        #region Display

        #region Tools

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

        #endregion

        #region Help

        private void AboutClicked(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/PierrePlaziat/MaGeek");
        }

        #endregion
    }

}
