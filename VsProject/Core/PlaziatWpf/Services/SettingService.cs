using System.IO;
using System.Text.Json;
using PlaziatTools;
using PlaziatTools.Data;

namespace PlaziatWpf.Services
{

    public class SettingService
    {

        private string path = Paths.File_Settings;
        
        public Dictionary<string, string> Settings { get; private set; } = new Dictionary<string, string>();

        public bool Init(string path)
        {
            this.path = path;
            if (!File.Exists(path))
            {
                Logger.Log("No settings" + path, LogLevels.Infos);
                SaveSettings();
                return false;
            }
            else
            {
                Logger.Log("Settings found : " + path, LogLevels.Infos);
                LoadSettings();
                return true;
            }
        }

        private void LoadSettings()
        {
            string jsonString = File.ReadAllText(path);
            try
            {
                Settings = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);
            }
            catch (Exception ex) { Logger.Log(ex); }
            if (Settings == null) Settings = new Dictionary<string, string>();
        }

        private void SaveSettings()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(Settings, options);
            File.WriteAllText(path, jsonString);
        }

        public void SetSetting(string key, string value)
        {
            Logger.Log(key + " - " + value);
            Settings[key] = value;
            SaveSettings();
        }

    }

}