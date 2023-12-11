using MaGeek.UI.Windows.Importers;
using MaGeek.UI.Windows.ImportExport;
using MageekService;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MaGeek.UI.Menus
{

    public partial class TopMenu : TemplatedUserControl
    {

        public TopMenu()
        {
            InitializeComponent();
            UpdateLangIcons(App.Config.Settings[Setting.ForeignLanguage]);
            UpdateCurrencyIcons(App.Config.Settings[Setting.Currency]);
        }

        #region Tools

        private void SetExplorer_Click(object sender, RoutedEventArgs e)
        {
            App.Events.RaiseLayoutAction(
                new AppEvents.LayoutEventArgs()
                {
                    EventType = AppEvents.LayoutEventType.OpenPanel,
                    information = "SetExplorer"
                }
            );
        }
        
        private void CardSearcher_Click(object sender, RoutedEventArgs e)
        {
            App.Events.RaiseLayoutAction(
                new AppEvents.LayoutEventArgs()
                {
                    EventType = AppEvents.LayoutEventType.OpenPanel,
                    information = "CardSearcher"
                }
            );
        }

        private void DeckList_Click(object sender, RoutedEventArgs e)
        {
            App.Events.RaiseLayoutAction(
                new AppEvents.LayoutEventArgs()
                {
                    EventType = AppEvents.LayoutEventType.OpenPanel,
                    information = "DeckList"
                }
            );
        }

        private void DeckContent_Click(object sender, RoutedEventArgs e)
        {
            App.Events.RaiseLayoutAction(
                new AppEvents.LayoutEventArgs()
                {
                    EventType = AppEvents.LayoutEventType.OpenPanel,
                    information = "DeckContent"
                }
            );
        }

        private void DeckTable_Click(object sender, RoutedEventArgs e)
        {
            App.Events.RaiseLayoutAction(
                new AppEvents.LayoutEventArgs()
                {
                    EventType = AppEvents.LayoutEventType.OpenPanel,
                    information = "DeckTable"
                }
            );
        }

        private void DeckStats_Click(object sender, RoutedEventArgs e)
        {
            App.Events.RaiseLayoutAction(
                new AppEvents.LayoutEventArgs()
                {
                    EventType = AppEvents.LayoutEventType.OpenPanel,
                    information = "DeckStats"
                }
            );
        }

        private void CardInspector_Click(object sender, RoutedEventArgs e)
        {
            App.Events.RaiseLayoutAction(
                new AppEvents.LayoutEventArgs()
                {
                    EventType = AppEvents.LayoutEventType.OpenPanel,
                    information = "CardInspector"
                }
            );
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

        #region Settings

        private void ChangeLanguage(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            App.Config.ChangeSetting(Setting.ForeignLanguage, item.Header.ToString());
            App.Events.RaiseUpdateCardCollec();
            UpdateLangIcons(item.Header.ToString());
        }

        private void UpdateLangIcons(string lang)
        {
            foreach (MenuItem v in LangBox.Items)
            {
                if (v.Header.ToString() == lang)
                {
                    v.Icon = new System.Windows.Controls.Image
                    {
                        Source = new BitmapImage(new Uri("/Resources/Images/TickOn.jpg", UriKind.Relative))
                    };
                }
                else
                {
                    v.Icon = new System.Windows.Controls.Image
                    {
                        Source = new BitmapImage(new Uri("/Resources/Images/TickOff.jpg", UriKind.Relative))
                    };
                }
            }
        }

        private void ChangeCurrency(object sender, RoutedEventArgs e)
        {
            MenuItem s = (MenuItem)sender;
            App.Config.ChangeSetting(Setting.Currency, s.Header.ToString());
            UpdateCurrencyIcons(s.Header.ToString());
        }

        private void UpdateCurrencyIcons(string currency)
        {
            foreach (MenuItem v in CurrencyBox.Items)
            {
                if (v.Header.ToString() == currency)
                {
                    v.Icon = new System.Windows.Controls.Image
                    {
                        Source = new BitmapImage(new Uri("/Resources/Images/TickOn.jpg", UriKind.Relative))
                    };
                }
                else
                {
                    v.Icon = new System.Windows.Controls.Image
                    {
                        Source = new BitmapImage(new Uri("/Resources/Images/TickOff.jpg", UriKind.Relative))
                    };
                }
            }
        }

        #endregion

        #region Layout

        private void LayoutBackup_Click(object sender, RoutedEventArgs e)
        {
            App.Events.RaiseLayoutAction(
                new AppEvents.LayoutEventArgs()
                {
                    EventType = AppEvents.LayoutEventType.Save,
                    information = "User"
                }
            );
        }
        private void LayoutRestore_Click(object sender, RoutedEventArgs e)
        {
            App.Events.RaiseLayoutAction(
                new AppEvents.LayoutEventArgs()
                {
                    EventType = AppEvents.LayoutEventType.Load,
                    information = "User"
                }
            );
        }

        private void ResetDefaultLayout(object sender, RoutedEventArgs e)
        {
            App.Events.RaiseLayoutAction(
                new AppEvents.LayoutEventArgs()
                {
                    EventType = AppEvents.LayoutEventType.Load,
                    information = "Default"
                }
            );
        }

        #endregion

        private void AboutClicked(object sender, RoutedEventArgs e)
        {
            App.HyperLink("https://github.com/PierrePlaziat/MaGeek");
        }

        private async void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            await MageekService.MageekService.ConvertCollectedFromScryfallIdToUuid();
        }
    }

}
