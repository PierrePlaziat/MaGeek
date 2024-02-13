using MageekCore.Tools;
using MageekFrontWpf.Framework.Services;

namespace MageekFrontWpf.AppValues
{
    public enum AppSetting
    {
        ForeignLanguage,
        Currency,
    };

    static class AppSettings
    {

        internal static void InitSettings(SettingService Settings)
        {
            if (!Settings.Settings.ContainsKey(AppSetting.ForeignLanguage)) Settings.Settings.Add(AppSetting.ForeignLanguage, "French");
            if (!Settings.Settings.ContainsKey(AppSetting.Currency)) Settings.Settings.Add(AppSetting.Currency, "Eur");
            Logger.Log("Done");
        }

    }

}
