using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.Framework.Services;
using PlaziatTools;
using System;
using MageekFrontWpf.Framework.AppValues;

namespace MageekFrontWpf.UI.ViewModels
{
    public partial class TopMenuViewModel
    {

        private WindowsService win;
        private SettingService conf;

        public TopMenuViewModel(WindowsService winManager, SettingService config)
        {
            win = winManager;
            conf = config;
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
            win.SaveLayout(obj);
        }

        [RelayCommand]
        private void LayoutRestore(string obj)
        {
            win.LoadLayout(obj);
        }

        [RelayCommand]
        private void ChangeLanguage(string lang)
        {
            conf.SetSetting(Setting.Translations, lang);
        }

        [RelayCommand]
        private void ChangeCurrency(string currency)
        {
            conf.SetSetting(Setting.Currency, currency);
        }

        [RelayCommand]
        private void About()
        {
            HttpUtils.OpenLink("https://github.com/PierrePlaziat/MaGeek");
        }

    }
}