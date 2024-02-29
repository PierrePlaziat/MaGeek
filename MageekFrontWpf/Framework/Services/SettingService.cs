using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using MageekCore.Data;
using PlaziatTools;
using MageekFrontWpf.Framework.AppValues;

namespace MageekFrontWpf.Framework.Services
{

    public class SettingService
    {

        private static string Path_Settings { get; } = Path.Combine(Folders.Roaming, "Settings.json");
        public Dictionary<Setting, string> Settings { get; private set; } = new Dictionary<Setting, string>();

        public SettingService()
        {
            AppValues.Settings.InitSettings(this);
            LoadSettings();
        }

        private void InitSettings(SettingService Settings)
        {
            if (!Settings.Settings.ContainsKey(Setting.ForeignLanguage)) Settings.Settings.Add(Setting.ForeignLanguage, "French");
            if (!Settings.Settings.ContainsKey(Setting.Currency)) Settings.Settings.Add(Setting.Currency, "Eur");
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
                Settings = JsonSerializer.Deserialize<Dictionary<Setting, string>>(jsonString);
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

        public void SetSetting(Setting key, string value)
        {
            Logger.Log(key + " - " + value);
            Settings[key] = value;
            SaveSettings();
        }

    }

}