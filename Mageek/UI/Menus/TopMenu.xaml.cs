using MaGeek.AppBusiness;
using MaGeek.Framework.Utils;
using MaGeek.UI.Windows.Importers;
using MaGeek.UI.Windows.ImportExport;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace MaGeek.UI.Menus
{

    public partial class TopMenu : TemplatedUserControl
    {

        public TopMenu()
        {
            InitializeComponent();
        }

        #region Tools

        private void SetExplorer_Click(object sender, RoutedEventArgs e)
        {
            App.Events.RaiseLayoutAction(LayoutEventType.Open_SetExplorer);
        }
        
        private void CardSearcher_Click(object sender, RoutedEventArgs e)
        {
            App.Events.RaiseLayoutAction(LayoutEventType.Open_CardSearcher);
        }

        private void DeckList_Click(object sender, RoutedEventArgs e)
        {
            App.Events.RaiseLayoutAction(LayoutEventType.Open_DeckList);
        }

        private void DeckContent_Click(object sender, RoutedEventArgs e)
        {
            App.Events.RaiseLayoutAction(LayoutEventType.Open_DeckContent);
        }

        private void DeckTable_Click(object sender, RoutedEventArgs e)
        {
            App.Events.RaiseLayoutAction(LayoutEventType.Open_DeckTable);
        }

        private void DeckStats_Click(object sender, RoutedEventArgs e)
        {
            App.Events.RaiseLayoutAction(LayoutEventType.Open_DeckStats);
        }

        private void CardInspector_Click(object sender, RoutedEventArgs e)
        {
            App.Events.RaiseLayoutAction(LayoutEventType.Open_CardInspector);
        }

        #endregion

        #region Utils

        private void OpenWindow_TxtImporter(object sender, RoutedEventArgs e)
        {
            var window = new TxtImporter();
            window.Show();
        }

        private void OpenWindow_PrecoImporter(object sender, RoutedEventArgs e)
        {
            var window = new PrecoImporter();
            window.Show();
        }

        private void OpenWindow_ProxyPrint(object sender, RoutedEventArgs e)
        {
            var window = new ProxyPrint(App.State.SelectedDeck);
            window.Show();
        }

        private void OpenWindow_CollectionEstimation(object sender, RoutedEventArgs e)
        {
            var window = new CollectionEstimation();
            window.Show();
        }

        #endregion

        #region Database

        private void UpdateCards(object sender, RoutedEventArgs e)
        {
            if (Log.AskUser("App will restart, proposing a card update."))
            {
                File.Delete(App.Config.Path_MtgJsonDownload_OldHash);
                App.Restart();
            }
        }

        private void BackupCollection(object sender, RoutedEventArgs e)
        {
            App.DB.Backup();
        }

        private void RestoreCollection(object sender, RoutedEventArgs e)
        {
            App.DB.PlanToRestore();
        }

        private void EraseDb(object sender, RoutedEventArgs e)
        {
            App.DB.PlanToErase();
        }

        #endregion

        #region Settings

        private void ChangeLanguage(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            App.Config.ChangeSetting(Setting.ForeignLanguage, item.Header.ToString());
            App.Events.RaiseUpdateCardCollec();
        }

        private void ChangeCurrency(object sender, RoutedEventArgs e)
        {
            MenuItem s = (MenuItem)sender;
            App.Config.ChangeSetting(Setting.Currency, s.Header.ToString());
        }

        #endregion

        #region Layout

        private void LayoutBackup_Click(object sender, RoutedEventArgs e)
        {
            App.Events.RaiseLayoutAction(LayoutEventType.Save);
        }
        private void LayoutRestore_Click(object sender, RoutedEventArgs e)
        {
            App.Events.RaiseLayoutAction(LayoutEventType.Load);
        }

        private void ResetDefaultLayout(object sender, RoutedEventArgs e)
        {
            App.Events.RaiseLayoutAction(LayoutEventType.ResetLayout);
        }

        #endregion

        private void AboutClicked(object sender, RoutedEventArgs e)
        {
            App.HyperLink("https://github.com/PierrePlaziat/MaGeek");
        }

    }

}
