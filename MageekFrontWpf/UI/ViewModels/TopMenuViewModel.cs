using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MageekFrontWpf.AppValues;
using MageekFrontWpf.Framework.Services;
using MageekServices.Tools;
using System;

namespace MageekFrontWpf.UI.ViewModels
{
    public partial class TopMenuViewModel
    {

        private WindowsService win;
        private SettingService conf;

        public TopMenuViewModel(WindowsService winManager, SettingService config)
        {
            this.win = winManager;
            this.conf = config;
        }

        [RelayCommand]
        private void OpenWindow(string window)
        {
            bool success = Enum.TryParse(window, out AppWindowEnum value);
            if (!success) return;
            win.OpenWindow(value);
        }

        [RelayCommand]
        private void OpenTool(string panel)
        {
            bool success = Enum.TryParse(panel, out AppToolsEnum value);
            if (!success) return;
            win.OpenTool(value);
        }

        [RelayCommand]
        private void LayoutBackup(string obj)
        {
            WeakReferenceMessenger.Default.Send(new SaveLayoutMessage("Default"));
        }

        [RelayCommand]
        private void LayoutRestore(string obj)
        {
            WeakReferenceMessenger.Default.Send(new LoadLayoutMessage("Default"));
        }

        [RelayCommand]
        private void ChangeLanguage(string lang)
        {
            conf.SetSetting(AppSetting.ForeignLanguage, lang);
        }

        [RelayCommand]
        private void ChangeCurrency(string currency)
        {
            conf.SetSetting(AppSetting.Currency, currency);
        }

        [RelayCommand]
        private void About()
        {
            HttpUtils.HyperLink("https://github.com/PierrePlaziat/MaGeek");
        }

    }
}