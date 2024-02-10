using MageekFrontWpf.Framework.Services;
using MageekCore.Data;
using MageekCore.Tools;
using System;
using System.Diagnostics;
using System.IO;

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
            Logger.Log("");
            if (!Settings.Settings.ContainsKey(AppSetting.ForeignLanguage)) Settings.Settings.Add(AppSetting.ForeignLanguage, "French");
            if (!Settings.Settings.ContainsKey(AppSetting.Currency)) Settings.Settings.Add(AppSetting.Currency, "Eur");
        }

    }

}
