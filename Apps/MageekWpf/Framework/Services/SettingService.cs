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

        private string Path_Settings { get; } = Path.Combine(Folders.Roaming, "Settings.json");
        public Dictionary<Setting, string> Settings { get; private set; } = new Dictionary<Setting, string>();

        public SettingService()
        {
            if (FirstLaunch()) SetDefaults();
            else LoadSettings();
        }

        private bool FirstLaunch()
        {
            bool isFirst = !File.Exists(Path_Settings);
            if (isFirst)  Logger.Log("Is first");
            return isFirst;
        }

        private void SetDefaults()
        {
            SetSetting(Setting.Translations, "French");
            SetSetting(Setting.Currency, "Eur");
            SaveSettings();
        }

        private void LoadSettings()
        {
            string jsonString = File.ReadAllText(Path_Settings);
            Settings = JsonSerializer.Deserialize<Dictionary<Setting, string>>(jsonString);
        }

        private void SaveSettings()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(Settings, options);
            File.WriteAllText(Path_Settings, jsonString);
        }

        public void SetSetting(Setting key, string value)
        {
            Logger.Log(key + " - " + value);
            Settings[key] = value;
            SaveSettings();
        }

    }

}