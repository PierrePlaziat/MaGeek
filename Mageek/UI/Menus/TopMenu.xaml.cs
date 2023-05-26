﻿using MaGeek.AppBusiness;
using MaGeek.UI.Windows.Importers;
using MaGeek.UI.Windows.ImportExport;
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

        #region Import Export

        private void OpenWindow_TxtImporter(object sender, RoutedEventArgs e)
        {
            var window = new TxtImporter();
            window.Show();
        }

        private void OpenWindow_SetImporter(object sender, RoutedEventArgs e)
        {
            var window = new SetExplorer();
            window.Show();
        }

        private void OpenWindow_PrecoImporter(object sender, RoutedEventArgs e)
        {
            var window = new PrecoImporter();
            window.Show();
        }

        private void OpenWindow_DeckListExporter(object sender, RoutedEventArgs e)
        {
            var window = new DeckListExporter(App.State.SelectedDeck);
            window.Show();
        }

        private void OpenWindow_ProxyPrint(object sender, RoutedEventArgs e)
        {
            var window = new ProxyPrint(App.State.SelectedDeck);
            window.Show();
        }

        #endregion

        #region Database

        private void BackupDb(object sender, RoutedEventArgs e)
        {
            App.DB.Backup();
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
            App.Config.ChangeSetting(Setting.ForeignLanguage, item.Header.ToString());
            App.Events.RaiseUpdateCardCollec();
        }

        #endregion

        #region Display

        #region Tools

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

        #endregion

        #region Help

        private void AboutClicked(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/PierrePlaziat/MaGeek");
        }

        #endregion

        private void ChangeCurrency(object sender, RoutedEventArgs e)
        {
            // TODO
            //App.Restart();
        }

        private void ReimportTraductions(object sender, RoutedEventArgs e)
        {
            MageekBulkinator.ReBulk_CardTraductions().ConfigureAwait(false);
        }
    }

}
