using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace MaGeek
{

    /// <summary>
    /// Storage gestion
    /// </summary>
    public class AppConfig
    {

        #region Attributes

        public bool SeemToBeFirstLaunch { get; private set; } = false;

        public static string Path_RoamingFolder { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MaGeek");
        public string Path_IllustrationsFolder { get; } = Path.Combine(Path_RoamingFolder, "CardsIllus");
        public string Path_SetIconsFolder { get; } = Path.Combine(Path_RoamingFolder, "SetIcons");
        public string Path_LayoutSave { get; } = Path.Combine(Path_RoamingFolder, "Layout.xml");
        private static string Path_Settings { get; } = Path.Combine(Path_RoamingFolder, "Settings.json");
        public Dictionary<Setting, string> Settings { get; private set; } = new Dictionary<Setting, string>();

        #endregion

        #region CTOR

        public AppConfig()
        {
            InitFolders();
            DefaultSettings();
            LoadSettings();
        }

        #endregion

        #region Methods

        private void InitFolders()
        {
            if (!File.Exists(Path_RoamingFolder))
            {
                SeemToBeFirstLaunch = true;
                Directory.CreateDirectory(Path_RoamingFolder);
            }
            if (!File.Exists(Path_IllustrationsFolder)) Directory.CreateDirectory(Path_IllustrationsFolder);
            if (!File.Exists(Path_SetIconsFolder)) Directory.CreateDirectory(Path_SetIconsFolder);
        }

        private void DefaultSettings()
        {
            Settings.Add(Setting.ForeignLanguage, "French");
            Settings.Add(Setting.Currency, "Eur");
        }

        public void ChangeSetting(Setting key, string value)
        {
            Settings[key] = value;
            SaveSettings();
        }

        private void SaveSettings()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(Settings, options);
            File.WriteAllText(Path_Settings, jsonString);
        }

        private void LoadSettings()
        {
            if (!File.Exists(Path_Settings))
            {
                SaveSettings();
                return;
            }
            string jsonString = File.ReadAllText(Path_Settings);
            Settings = JsonSerializer.Deserialize<Dictionary<Setting, string>>(jsonString);
        }

        #endregion

    }

    public enum Setting
    {
        ForeignLanguage,
        Currency,
    };

}