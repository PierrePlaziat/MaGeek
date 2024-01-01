using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace MageekFrontWpf
{

    public class UserSettings
    {

        private static string Path_Settings { get; } = Path.Combine(MageekService.Folders.Roaming, "Settings.json");
        public Dictionary<UserSetting, string> Settings { get; private set; } = new Dictionary<UserSetting, string>();

        public UserSettings()
        {
            SetDefaultSettings();
            LoadSettings();
        }

        private void SetDefaultSettings()
        {
            Settings.Add(UserSetting.ForeignLanguage, "French");
            Settings.Add(UserSetting.Currency, "Eur");
        }

        private void LoadSettings()
        {
            if (!File.Exists(Path_Settings))
            {
                SaveSettings();
                return;
            }
            string jsonString = File.ReadAllText(Path_Settings);
            Settings = JsonSerializer.Deserialize<Dictionary<UserSetting, string>>(jsonString);
        }

        private void SaveSettings()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(Settings, options);
            File.WriteAllText(Path_Settings, jsonString);
        }

        public void SetSetting(UserSetting key, string value)
        {
            Settings[key] = value;
            SaveSettings();
        }

    }

    public enum UserSetting
    {
        ForeignLanguage,
        Currency,
    };

}