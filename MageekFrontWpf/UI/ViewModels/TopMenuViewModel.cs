using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.App;
using MageekFrontWpf.Framework.Services;
using MageekService.Tools;
using System;

namespace MageekFrontWpf.UI.ViewModels
{
    public partial class TopMenuViewModel
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

        [RelayCommand]
        private void OpenWindow(string window)
        {
            bool success = Enum.TryParse(window, out AppWindowEnum value);
            if (!success) return;
            winManager.OpenWindow(value);
        }

        [RelayCommand]
        private void OpenPanel(string panel)
        {
            bool success = Enum.TryParse(panel, out AppPanelEnum value);
            if (!success) return;
            winManager.OpenPanel(value);
        }

        [RelayCommand]
        private void LayoutBackup(string obj)
        {
            events.RaiseLayoutAction(
                new AppEvents.LayoutEventArgs()
                {
                    EventType = AppEvents.LayoutEventType.Save,
                    //information = "User"
                }
            );
        }

        [RelayCommand]
        private void LayoutRestore(string obj)
        {
            events.RaiseLayoutAction(
                new AppEvents.LayoutEventArgs()
                {
                    EventType = AppEvents.LayoutEventType.Load,
                    //information = "User"
                }
            );
        }

        [RelayCommand]
        private void ChangeLanguage(string lang)
        {
            config.SetSetting(AppSetting.ForeignLanguage, lang);
        }

        [RelayCommand]
        private void ChangeCurrency(string currency)
        {
            config.SetSetting(AppSetting.Currency, currency);
        }

        [RelayCommand]
        private void About()
        {
            HttpUtils.HyperLink("https://github.com/PierrePlaziat/MaGeek");
        }

        #region Not used

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

    }
}