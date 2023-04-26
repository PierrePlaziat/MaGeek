using System.IO;
using System;
using System.Text.Json;
using System.Collections.Generic;

namespace MaGeek.AppFramework
{
    public enum Setting {
        ForeignLangugage,
        Currency, //TODO link
    };

    /// <summary>
    /// Storage gestion
    /// </summary>
    public class AppConfig
    {

        private static readonly string Path_RoamingFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MaGeek");
        private static readonly string Path_Settings = Path.Combine(Path_RoamingFolder, "Settings.json");

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

        internal static string[] GetSqliteDbCreationString()
        {
            return new string[] {
                "CREATE TABLE \"CardRelations\" (\r\n\t\"RelationId\"\tINTEGER,\r\n\t\"Card1Id\"\tTEXT,\r\n\t\"Card2Id\"\tTEXT,\r\n\t\"LastUpdate\"\tTEXT,\r\n\t\"RelationType\"\tTEXT,\r\n\tPRIMARY KEY(\"RelationId\")\r\n);",
                "CREATE TABLE \"CardTraductions\" (\r\n\t\"TraductionId\"\tINTEGER,\r\n\t\"CardId\"\tTEXT,\r\n\t\"Language\"\tTEXT,\r\n\t\"TraductedName\"\tTEXT,\r\n\tPRIMARY KEY(\"TraductionId\")\r\n);",
                "CREATE TABLE \"CardValues\" (\r\n\t\"MultiverseId\"\tTEXT,\r\n\t\"LastUpdate\"\tTEXT,\r\n\t\"ValueEur\"\tTEXT,\r\n\t\"ValueUsd\"\tTEXT,\r\n\t\"EdhRecRank\"\tINTEGER,\r\n\tPRIMARY KEY(\"MultiverseId\")\r\n);",
                "CREATE TABLE \"CardVariants\" (\r\n\t\"Id\"\tTEXT,\r\n\t\"MultiverseId\"\tTEXT,\r\n\t\"ImageUrl_Front\"\tTEXT,\r\n\t\"Rarity\"\tTEXT,\r\n\t\"SetName\"\tTEXT,\r\n\t\"CardId\"\tTEXT,\r\n\t\"IsCustom\"\tINTEGER,\r\n\t\"CustomName\"\tTEXT,\r\n\t\"Got\"\tINTEGER,\r\n\t\"LastUpdate\"\tTEXT,\r\n\t\"Artist\"\tTEXT,\r\n\t\"EdhRecRank\"\tINTEGER,\r\n\t\"ImageUrl_Back\"\tTEXT,\r\n\t\"Lang\"\tTEXT,\r\n\t\"TraductedText\"\tTEXT,\r\n\t\"TraductedTitle\"\tTEXT,\r\n\t\"TraductedType\"\tINTEGER,\r\n\t\"ValueEur\"\tTEXT,\r\n\t\"ValueUsd\"\tTEXT\r\n);",
                "CREATE TABLE \"Cards\" (\r\n\t\"CardId\"\tTEXT,\r\n\t\"Type\"\tTEXT,\r\n\t\"ManaCost\"\tTEXT,\r\n\t\"Cmc\"\tINTEGER,\r\n\t\"Text\"\tTEXT,\r\n\t\"Power\"\tTEXT,\r\n\t\"Toughness\"\tINTEGER,\r\n\t\"FavouriteVariant\"\tTEXT,\r\n\t\"ColorIdentity\"\tTEXT,\r\n\t\"LastUpdate\"\tTEXT\r\n);",
                "CREATE TABLE \"CardsInDecks\" (\r\n\t\"DeckId\"\tINTEGER,\r\n\t\"CardId\"\tTEXT,\r\n\t\"Quantity\"\tINTEGER,\r\n\t\"RelationType\"\tINTEGER,\r\n\tPRIMARY KEY(\"CardId\",\"DeckId\")\r\n);",
                "CREATE TABLE \"Decks\" (\r\n\t\"DeckId\"\tINTEGER,\r\n\t\"Title\"\tTEXT,\r\n\t\"Description\"\tTEXT,\r\n\t\"DeckColors\"\tTEXT,\r\n\t\"CardCount\"\tINTEGER,\r\n\tPRIMARY KEY(\"DeckId\")\r\n);",
                "CREATE TABLE \"Legalities\" (\r\n\t\"LegalityId\"\tINTEGER,\r\n\t\"CardId\"\tTEXT,\r\n\t\"LastUpdate\"\tTEXT,\r\n\t\"Format\"\tTEXT,\r\n\t\"IsLegal\"\tTEXT,\r\n\tPRIMARY KEY(\"LegalityId\")\r\n);",
                "CREATE TABLE \"Params\" (\r\n\t\"ParamName\"\tTEXT,\r\n\t\"ParamValue\"\tTEXT\r\n);",
                "CREATE TABLE \"Tags\" (\r\n\t\"Id\"\tINTEGER,\r\n\t\"Tag\"\tTEXT,\r\n\t\"CardId\"\tINTEGER,\r\n\tPRIMARY KEY(\"Id\")\r\n);",
            };
        }

        #endregion
    }

}