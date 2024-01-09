using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace MageekFrontWpf
{

    public class SettingService
    {

        private static string Path_Settings { get; } = Path.Combine(MageekService.Folders.Roaming, "Settings.json");
        public Dictionary<Settings, string> Settings { get; private set; } = new Dictionary<Settings, string>();

        public SettingService()
        {
            SetDefaultSettings();
            LoadSettings();
        }

        private void SetDefaultSettings()
        {
            Settings.Add(MageekFrontWpf.Settings.ForeignLanguage, "French");
            Settings.Add(MageekFrontWpf.Settings.Currency, "Eur");
        }

        private void LoadSettings()
        {
            if (!File.Exists(Path_Settings))
            {
                SaveSettings();
                return;
            }
            string jsonString = File.ReadAllText(Path_Settings);
            Settings = JsonSerializer.Deserialize<Dictionary<Settings, string>>(jsonString);
        }

        private void SaveSettings()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(Settings, options);
            File.WriteAllText(Path_Settings, jsonString);
        }

        public void SetSetting(Settings key, string value)
        {
            Settings[key] = value;
            SaveSettings();
        }

    }

    public enum Settings
    {
        ForeignLanguage,
        Currency,
    };

}