using System.IO;
using System;
using System.Text.Json;
using System.Collections.Generic;

namespace MaGeek.AppFramework
{
    public enum Setting { ForeignLangugage };

    /// <summary>
    /// Storage gestion
    /// </summary>
    public class AppConfig
    {

        private static string Path_RoamingFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MaGeek");
        private static string Path_Settings = Path.Combine(Path_RoamingFolder, "Settings.json");

        public string Path_ImageFolder { get; } = Path.Combine(Path_RoamingFolder, "CardsIllus");
        public string Path_Db { get; } = Path.Combine(Path_RoamingFolder, "MaGeek.db");
        public string Path_ImporterState { get; } = Path.Combine(Path_RoamingFolder, "ImporterState.txt"); 
        public string Path_LayoutSave { get; } = Path.Combine(Path_RoamingFolder, "Layout.txt");

        public Dictionary<Setting, string> Settings { get; private set; } = new Dictionary<Setting, string>();

        public AppConfig()
        {
            InitFolders();
            DefaultConfig();
            LoadConfig();
        }

        private void DefaultConfig()
        {
            Settings.Add(Setting.ForeignLangugage, "French");
        }
        #region Methods

        private void InitFolders()
        {
            if (!File.Exists(Path_RoamingFolder)) Directory.CreateDirectory(Path_RoamingFolder);
            if (!File.Exists(Path_ImageFolder)) Directory.CreateDirectory(Path_ImageFolder);
        }

        public void ChangeSetting(Setting key, string value)
        {
            Settings[key] = value;
            Save();
        }

        private void Save()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(Settings, options);
            File.WriteAllText(Path_Settings, jsonString);
        }

        private void LoadConfig()
        {
            if (!File.Exists(Path_Settings))
            {
                Save();
                return;
            }
            string jsonString = File.ReadAllText(Path_Settings);
            Settings = JsonSerializer.Deserialize<Dictionary<Setting, string>>(jsonString);
        }

        #endregion
    }

}