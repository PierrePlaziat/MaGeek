using System;

namespace MageekFrontWpf.App
{
    public enum AppSetting
    {
        ForeignLanguage,
        Currency,
    };

    static class AppSettings
    {
        internal static void SetDefaultSettings(SettingService Settings)
        {
            Settings.Settings.Add(AppSetting.ForeignLanguage, "French");
            Settings.Settings.Add(AppSetting.Currency, "Eur");
        }

    }

}
