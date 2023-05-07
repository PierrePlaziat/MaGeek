using System.IO;
using System;
using System.Text.Json;
using System.Collections.Generic;

namespace MaGeek.AppFramework
{

    /// <summary>
    /// Storage gestion
    /// </summary>
    public class AppConfig
    {

        #region Attributes

        private static readonly string Path_RoamingFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MaGeek");
        private static readonly string Path_Settings = Path.Combine(Path_RoamingFolder, "Settings.json");

        public string Path_ImageFolder { get; } = Path.Combine(Path_RoamingFolder, "CardsIllus");
        public string Path_Db { get; } = Path.Combine(Path_RoamingFolder, "MaGeek.db");
        public string Path_ImporterState { get; } = Path.Combine(Path_RoamingFolder, "ImporterState.txt"); 
        public string Path_LayoutSave { get; } = Path.Combine(Path_RoamingFolder, "Layout.txt");
        public string Path_MtgJsonDownload { get; } = Path.Combine(Path_RoamingFolder, "mtgjson.sqlite");

        public Dictionary<Setting, string> Settings { get; private set; } = new Dictionary<Setting, string>();

        public bool seemToBeFirstLaunch { get; private set; } = false;

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
            if (!File.Exists(Path_Db))
            {
                seemToBeFirstLaunch = true;
                Directory.CreateDirectory(Path_RoamingFolder);
            }
            if (!File.Exists(Path_ImageFolder)) Directory.CreateDirectory(Path_ImageFolder);
        }

        private void DefaultSettings()
        {
            Settings.Add(Setting.ForeignLangugage, "French");
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
        ForeignLangugage,
        Currency,
    };

}