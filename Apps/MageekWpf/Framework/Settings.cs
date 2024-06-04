using PlaziatCore;
using PlaziatWpf.Services;

namespace MageekFrontWpf.Framework
{
    public enum Setting
    {
        Translations,
        Currency,
    };

    static class Settings
    {

        internal static void InitSettings(SettingService Settings)
        {
            if (!Settings.Settings.ContainsKey("Translations")) Settings.Settings.Add("Translations", "French");
            if (!Settings.Settings.ContainsKey("Currency")) Settings.Settings.Add("Currency", "Eur");
            Logger.Log("Done");
        }

    }

}
