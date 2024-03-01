using PlaziatTools;
using MageekFrontWpf.Framework.Services;

namespace MageekFrontWpf.Framework.AppValues
{
    public enum Setting
    {
        ForeignLanguage,
        Currency,
    };

    static class Settings
    {

        internal static void InitSettings(SettingService Settings)
        {
            if (!Settings.Settings.ContainsKey(Setting.ForeignLanguage)) Settings.Settings.Add(Setting.ForeignLanguage, "French");
            if (!Settings.Settings.ContainsKey(Setting.Currency)) Settings.Settings.Add(Setting.Currency, "Eur");
            Logger.Log("Done");
        }

    }

}
