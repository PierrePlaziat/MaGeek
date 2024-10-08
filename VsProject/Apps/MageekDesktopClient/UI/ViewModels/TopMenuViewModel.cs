﻿using CommunityToolkit.Mvvm.Input;
using PlaziatWpf.Services;
using PlaziatTools;
using System;
using MageekDesktopClient.Framework;

namespace MageekDesktopClient.UI.ViewModels
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
            win.OpenPanel(value.ToString());
        }

        [RelayCommand]
        private void LayoutBackup(string layoutName)
        {
            win.SaveLayout(layoutName);
        }

        [RelayCommand]
        private void LayoutRestore(string obj)
        {
            App.LoadLayout().ConfigureAwait(false);
        }

        [RelayCommand]
        private void ChangeLanguage(string lang)
        {
            conf.SetSetting(Setting.Translations.ToString(), lang);
        }

        [RelayCommand]
        private void ChangeCurrency(string currency)
        {
            conf.SetSetting(Setting.Currency.ToString(), currency);
        }

        [RelayCommand]
        private void About()
        {
            HttpUtils.OpenLink("https://github.com/PierrePlaziat/MaGeek");
        }

    }
}