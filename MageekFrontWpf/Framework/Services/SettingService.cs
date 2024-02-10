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
            Logger.Log("");
            if (!Settings.Settings.ContainsKey(AppSetting.ForeignLanguage)) Settings.Settings.Add(AppSetting.ForeignLanguage, "French");
            if (!Settings.Settings.ContainsKey(AppSetting.Currency)) Settings.Settings.Add(AppSetting.Currency, "Eur");
        }

        private void LoadSettings()
        {
            Logger.Log("");
            if (!File.Exists(Path_Settings))
            {
                SaveSettings();
                return;
            }
            string jsonString = File.ReadAllText(Path_Settings);
            Settings = JsonSerializer.Deserialize<Dictionary<AppSetting, string>>(jsonString);
        }

        private void SaveSettings()
        {
            Logger.Log(DateTime.Now.ToString());
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(Settings, options);
            File.WriteAllText(Path_Settings, jsonString);
        }

        public void SetSetting(AppSetting key, string value)
        {
            Logger.Log(DateTime.Now.ToString() + key + " - " + value);
            Settings[key] = value;
            SaveSettings();
        }

    }

}