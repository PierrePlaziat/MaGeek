using MaGeek.Events;
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
            var window = new TxtImporter() { Width = 310 };
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
            App.DB.BackupDb();
        }

        private void RestoreDb(object sender, RoutedEventArgs e)
        {
            App.DB.RestoreDb();
        }

        private void EraseDb(object sender, RoutedEventArgs e)
        {
            App.DB.EraseDb();
        }

        #endregion

        #region Language

        private void ChangeLanguage(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            App.LANG.SetForeignLanguage(item.Header.ToString());
        }

        #endregion

        #region Display

        #region Tools

        private void CardSearcher_Click(object sender, RoutedEventArgs e)
        {
            App.STATE.RaiseLayoutAction(LayoutEventType.Open_CardSearcher);
        }

        private void DeckList_Click(object sender, RoutedEventArgs e)
        {
            App.STATE.RaiseLayoutAction(LayoutEventType.Open_DeckList);
        }

        private void DeckContent_Click(object sender, RoutedEventArgs e)
        {
            App.STATE.RaiseLayoutAction(LayoutEventType.Open_DeckContent);
        }

        private void DeckTable_Click(object sender, RoutedEventArgs e)
        {
            App.STATE.RaiseLayoutAction(LayoutEventType.Open_DeckTable);
        }

        private void DeckStats_Click(object sender, RoutedEventArgs e)
        {
            App.STATE.RaiseLayoutAction(LayoutEventType.Open_DeckStats);
        }

        private void CardInspector_Click(object sender, RoutedEventArgs e)
        {
            App.STATE.RaiseLayoutAction(LayoutEventType.Open_CardInspector);
        }

        #endregion

        #region Layout

        private void LayoutBackup_Click(object sender, RoutedEventArgs e)
        {
            App.STATE.RaiseLayoutAction(LayoutEventType.Save);
        }
        private void LayoutRestore_Click(object sender, RoutedEventArgs e)
        {
            App.STATE.RaiseLayoutAction(LayoutEventType.Load);
        }

        private void ResetDefaultLayout(object sender, RoutedEventArgs e)
        {
            App.STATE.RaiseLayoutAction(LayoutEventType.ResetLayout);
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
