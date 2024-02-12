using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using MageekFrontWpf.AppValues;
using MageekCore.Data;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System;
using MageekCore.Tools;

namespace MageekFrontWpf.Framework.Services
{

    public class SettingService
    {

        private static string Path_Settings { get; } = Path.Combine(Folders.Roaming, "Settings.json");
        public Dictionary<AppSetting, string> Settings { get; private set; } = new Dictionary<AppSetting, string>();

        public SettingService()
        {
            AppSettings.InitSettings(this);
            LoadSettings();
        }

        private void InitSettings(SettingService Settings)
        {
            if (!Settings.Settings.ContainsKey(AppSetting.ForeignLanguage)) Settings.Settings.Add(AppSetting.ForeignLanguage, "French");
            if (!Settings.Settings.ContainsKey(AppSetting.Currency)) Settings.Settings.Add(AppSetting.Currency, "Eur");
            Logger.Log("Done");
        }

        private void LoadSettings()
        {
            if (!File.Exists(Path_Settings))
            {
                Logger.Log("No settings found");
                SaveSettings();
            }
            else
            {
                string jsonString = File.ReadAllText(Path_Settings);
                Settings = JsonSerializer.Deserialize<Dictionary<AppSetting, string>>(jsonString);
                Logger.Log("Done");
            }
        }

        private void SaveSettings()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(Settings, options);
            File.WriteAllText(Path_Settings, jsonString);
            Logger.Log("Done");
        }

        public void SetSetting(AppSetting key, string value)
        {
            Logger.Log(key + " - " + value);
            Settings[key] = value;
            SaveSettings();
        }

    }

}