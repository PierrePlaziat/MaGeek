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

        internal string[] GetSqliteDbCreationString()
        {
            return new string[] {
                "CREATE TABLE \"Params\" (\r\n\t\"ParamName\"\tTEXT,\r\n\t\"ParamValue\"\tTEXT\r\n)",
                "CREATE TABLE \"Prices\" (\r\n\t\"MultiverseId\"\tTEXT,\r\n\t\"LastUpdate\"\tTEXT,\r\n\t\"Value\"\tTEXT,\r\n\tPRIMARY KEY(\"MultiverseId\")\r\n)",
                "CREATE TABLE \"Legalities\" (\r\n\t\"Id\"\tINTEGER,\r\n\t\"MultiverseId\"\tTEXT,\r\n\t\"LastUpdate\"\tTEXT,\r\n\t\"Format\"\tTEXT,\r\n\t\"IsLegal\"\tTEXT,\r\n\tPRIMARY KEY(\"Id\")\r\n)",
                "CREATE TABLE \"Tags\" (\r\n\t\"Id\"\tINTEGER,\r\n\t\"Tag\"\tTEXT,\r\n\t\"CardId\"\tINTEGER,\r\n\tPRIMARY KEY(\"Id\")\r\n)",
                "CREATE TABLE \"cardVariants\" (\r\n\t\"Id\"\tTEXT,\r\n\t\"MultiverseId\"\tTEXT,\r\n\t\"ImageUrl\"\tTEXT,\r\n\t\"Rarity\"\tTEXT,\r\n\t\"SetName\"\tINTEGER,\r\n\t\"CardId\"\tTEXT,\r\n\t\"IsCustom\"\tINTEGER,\r\n\t\"CustomName\"\tTEXT,\r\n\t\"Got\"\tINTEGER\r\n)",
                "CREATE TABLE \"cards\" (\r\n\t\"CardId\"\tTEXT,\r\n\t\"Type\"\tTEXT,\r\n\t\"ManaCost\"\tTEXT,\r\n\t\"Cmc\"\tINTEGER,\r\n\t\"Text\"\tTEXT,\r\n\t\"Power\"\tTEXT,\r\n\t\"Toughness\"\tTEXT,\r\n\t\"FavouriteVariant\"\tTEXT\r\n)",
                "CREATE TABLE \"cardsInDecks\" (\r\n\t\"DeckId\"\tINTEGER,\r\n\t\"CardId\"\tTEXT,\r\n\t\"Quantity\"\tINTEGER,\r\n\t\"RelationType\"\tINTEGER,\r\n\tPRIMARY KEY(\"CardId\",\"DeckId\")\r\n)",
                "CREATE TABLE \"decks\" (\r\n\t\"DeckId\"\tINTEGER,\r\n\t\"Title\"\tTEXT,\r\n\tPRIMARY KEY(\"DeckId\")\r\n)",
                "CREATE TABLE \"traductions\" (\r\n\t\"TraductionId\"\tINTEGER,\r\n\t\"CardId\"\tTEXT,\r\n\t\"Language\"\tTEXT,\r\n\t\"TraductedName\"\tTEXT,\r\n\tPRIMARY KEY(\"TraductionId\")\r\n)",
            };
        }

        #endregion
    }

}