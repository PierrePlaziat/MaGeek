﻿using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using MageekFrontWpf.App;

namespace MageekFrontWpf
{

    public class SettingService
    {

        private static string Path_Settings { get; } = Path.Combine(MageekService.Folders.Roaming, "Settings.json");
        public Dictionary<AppSetting, string> Settings { get; private set; } = new Dictionary<AppSetting, string>();

        public SettingService()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
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
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(Settings, options);
            File.WriteAllText(Path_Settings, jsonString);
        }

        public void SetSetting(AppSetting key, string value)
        {
            Settings[key] = value;
            SaveSettings();
        }

    }

}