using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.App;
using MageekFrontWpf.Framework.Services;
using MageekService.Tools;
using System;
using System.Windows.Input;

namespace MageekFrontWpf.UI.ViewModels
{
    public class TopMenuViewModel
    {

        private WindowsManager winManager;
        private AppEvents events;
        private SettingService config;

        public TopMenuViewModel(WindowsManager winManager, AppEvents events, SettingService config)
        {
            this.winManager = winManager;
            this.events = events;
            this.config = config;
        }

        public ICommand OpenWindowCommand { get; } = new RelayCommand<string>(OpenWindow);
        public ICommand OpenPanelCommand { get; } = new RelayCommand<string>(OpenPanel);
        public ICommand LayoutBackupCommand { get; } = new RelayCommand<string>(LayoutBackup);

        private static void LayoutBackup(string obj)
        {
            throw new NotImplementedException();
        }

        public ICommand LayoutRestoreCommand { get; } = new RelayCommand<string>(LayoutRestore);

        private static void LayoutRestore(string obj)
        {
            throw new NotImplementedException();
        }

        public ICommand ChangeLanguageCommand { get; } = new RelayCommand(ChangeLanguage);

        private static void ChangeLanguage()
        {
            throw new NotImplementedException();
        }

        public ICommand ChangeCurrencyCommand { get; } = new RelayCommand(ChangeCurrency);

        private static void ChangeCurrency()
        {
            throw new NotImplementedException();
        }

        public ICommand AboutCommand { get; } = new RelayCommand(About);

        private static void About()
        {
            throw new NotImplementedException();
        }

        private static void OpenWindow(string obj)
        {
            throw new NotImplementedException();
        }

        private static void OpenPanel(string obj)
        {
            throw new NotImplementedException();
        }

        #region Tools

        private void SetExplorer_Click()
        {
            events.RaiseLayoutAction(
                new AppEvents.LayoutEventArgs()
                {
                    EventType = AppEvents.LayoutEventType.OpenPanel,
                    information = "SetExplorer"
                }
            );
        }

        private void CardSearcher_Click()
        {
            events.RaiseLayoutAction(
                new AppEvents.LayoutEventArgs()
                {
                    EventType = AppEvents.LayoutEventType.OpenPanel,
                    information = "CardSearcher"
                }
            );
        }

        private void DeckList_Click()
        {
            events.RaiseLayoutAction(
                new AppEvents.LayoutEventArgs()
                {
                    EventType = AppEvents.LayoutEventType.OpenPanel,
                    information = "DeckList"
                }
            );
        }

        private void DeckContent_Click()
        {
            events.RaiseLayoutAction(
                new AppEvents.LayoutEventArgs()
                {
                    EventType = AppEvents.LayoutEventType.OpenPanel,
                    information = "DeckContent"
                }
            );
        }

        private void DeckTable_Click()
        {
            events.RaiseLayoutAction(
                new AppEvents.LayoutEventArgs()
                {
                    EventType = AppEvents.LayoutEventType.OpenPanel,
                    information = "DeckTable"
                }
            );
        }

        private void DeckStats_Click()
        {
            events.RaiseLayoutAction(
                new AppEvents.LayoutEventArgs()
                {
                    EventType = AppEvents.LayoutEventType.OpenPanel,
                    information = "DeckStats"
                }
            );
        }

        private void CardInspector_Click()
        {
            events.RaiseLayoutAction(
                new AppEvents.LayoutEventArgs()
                {
                    EventType = AppEvents.LayoutEventType.OpenPanel,
                    information = "CardInspector"
                }
            );
        }

        #endregion

        #region Utils

        private void OpenWindow_TxtImporter()
        {
            winManager.OpenWindow(AppWindowEnum.Import);
        }

        private void OpenWindow_PrecoImporter()
        {
            winManager.OpenWindow(AppWindowEnum.Precos);
        }

        private void OpenWindow_ProxyPrint()
        {
            winManager.OpenWindow(AppWindowEnum.Print);
        }

        private void OpenWindow_CollectionEstimation()
        {
            winManager.OpenWindow(AppWindowEnum.Estimation);
        }

        #endregion

        #region Settings

        private void ChangeLanguage(string newLang)
        {
            config.SetSetting(AppSetting.ForeignLanguage, newLang);
            events.RaiseUpdateCardCollec();
            //UpdateLangIcons(newLang);
        }

        //private void UpdateLangIcons(string lang)
        //{
        //    foreach (MenuItem v in LangBox.Items)
        //    {
        //        if (v.Header.ToString() == lang)
        //        {
        //            v.Icon = new System.Windows.Controls.Image
        //            {
        //                Source = new BitmapImage(new Uri("/Resources/Images/TickOn.jpg", UriKind.Relative))
        //            };
        //        }
        //        else
        //        {
        //            v.Icon = new System.Windows.Controls.Image
        //            {
        //                Source = new BitmapImage(new Uri("/Resources/Images/TickOff.jpg", UriKind.Relative))
        //            };
        //        }
        //    }
        //}

        private void ChangeCurrency(string newCurrency)
        {
            config.SetSetting(AppSetting.Currency, newCurrency);
            //UpdateCurrencyIcons(newCurrency);
        }

        //private void UpdateCurrencyIcons(string currency)
        //{
        //    foreach (MenuItem v in CurrencyBox.Items)
        //    {
        //        if (v.Header.ToString() == currency)
        //        {
        //            v.Icon = new System.Windows.Controls.Image
        //            {
        //                Source = new BitmapImage(new Uri("/Resources/Images/TickOn.jpg", UriKind.Relative))
        //            };
        //        }
        //        else
        //        {
        //            v.Icon = new System.Windows.Controls.Image
        //            {
        //                Source = new BitmapImage(new Uri("/Resources/Images/TickOff.jpg", UriKind.Relative))
        //            };
        //        }
        //    }
        //}

        #endregion

        #region Layout

        private void LayoutBackup_Click()
        {
            events.RaiseLayoutAction(
                new AppEvents.LayoutEventArgs()
                {
                    EventType = AppEvents.LayoutEventType.Save,
                    information = "User"
                }
            );
        }
        private void LayoutRestore_Click()
        {
            events.RaiseLayoutAction(
                new AppEvents.LayoutEventArgs()
                {
                    EventType = AppEvents.LayoutEventType.Load,
                    information = "User"
                }
            );
        }

        private void ResetDefaultLayout()
        {
            events.RaiseLayoutAction(
                new AppEvents.LayoutEventArgs()
                {
                    EventType = AppEvents.LayoutEventType.Load,
                    information = "Default"
                }
            );
        }

        #endregion

        private void AboutClicked()
        {
            HttpUtils.HyperLink("https://github.com/PierrePlaziat/MaGeek");
        }

        private async void MenuItem_Click()
        {
            await MageekService.MageekService.ConvertCollectedFromScryfallIdToUuid();
        }

    }
}