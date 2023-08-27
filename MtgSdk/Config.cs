using Microsoft.Data.Sqlite;
using System.IO;

namespace MageekSdk
{
    public class Config
    {

        public static string Url_MtgjsonHash { get; } = "https://mtgjson.com/api/v5/AllPrintings.sqlite.sha256";
        public static string Url_MtgjsonData { get; } = "https://mtgjson.com/api/v5/AllPrintings.sqlite";
        public static string Path_RoamingFolder { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MaGeek");
        public static string Path_SDK { get; } = Path.Combine(Path_RoamingFolder, "SDK");
        public static string Path_DbFolder { get; } = Path.Combine(Path_SDK, "DB");
        public static string Path_Db { get; } = Path.Combine(Path_DbFolder, "MaGeek.db");
        public static string Path_MtgJsonDownload { get; } = Path.Combine(Path_DbFolder, "mtgjson.sqlite");
        public static string Path_MtgJsonDownload_NewHash { get; } = Path.Combine(Path_DbFolder, "mtgjson.sqlite.sha256");
        public static string Path_MtgJsonDownload_OldHash { get; } = Path.Combine(Path_DbFolder, "mtgjson.sqlite.sha256_old");
        public static string Path_IllustrationsFolder { get; } = Path.Combine(Path_SDK, "CardsIllus");

        #region Debug
        // Here just in case,
        // can be usefull when the endpoint is down for example,
        // Make sure to reset to false after usage.
        public static bool bypass_Hash = false;
        public static bool bypass_Data = false;
        #endregion

        public static string[] TablesCreationString { get; } = new string[] {
            "CREATE TABLE \"CardArchetypes\" (\r\n\t\"CardUuid\"\tTEXT,\r\n\t\"ArchetypeId\"\tTEXT\r\n);",
            "CREATE TABLE \"CardTraductions\" (\r\n\t\"CardUuid\"\tTEXT,\r\n\t\"Language\"\tTEXT,\r\n\t\"Traduction\"\tTEXT,\r\n\t\"NormalizedTraduction\"\tTEXT\r\n);",
            "CREATE TABLE \"CollectedCard\" (\r\n\t\"CardUuid\"\tTEXT,\r\n\t\"Collected\"\tINTEGER\r\n);",
            "CREATE TABLE \"Decks\" (\r\n\t\"DeckId\"\tTEXT,\r\n\t\"Title\"\tTEXT,\r\n\t\"Description\"\tTEXT,\r\n\t\"DeckColors\"\tTEXT,\r\n\t\"CardCount\"\tINTEGER\r\n);",
            "CREATE TABLE \"DeckCard\" (\r\n\t\"DeckId\"\tTEXT,\r\n\t\"CardUuid\"\tTEXT,\r\n\t\"Quantity\"\tINTEGER,\r\n\t\"RelationType\"\tINTEGER\r\n);",
            "CREATE TABLE \"FavVariant\" (\r\n\t\"ArchetypeId\"\tTEXT,\r\n\t\"FavUuid\"\tTEXT\r\n);",
            "CREATE TABLE \"Param\" (\r\n\t\"ParamName\"\tTEXT,\r\n\t\"ParamValue\"\tTEXT\r\n);",
            "CREATE TABLE \"PriceLine\" (\r\n\t\"CardUuid\"\tTEXT,\r\n\t\"LastUpdate\"\tTEXT,\r\n\t\"PriceEur\"\tTEXT,\r\n\t\"PriceUsd\"\tTEXT,\r\n\t\"EdhrecScore\"\tINTEGER\r\n);",
            "CREATE TABLE \"Tag\" (\r\n\t\"TagId\"\tTEXT,\r\n\t\"TagContent\"\tTEXT,\r\n\t\"ArchetypeId\"\tTEXT\r\n);"
        };

        internal static void InitFolders()
        {
            if (!File.Exists(Path_RoamingFolder)) Directory.CreateDirectory(Path_RoamingFolder);
            if (!File.Exists(Path_SDK)) Directory.CreateDirectory(Path_SDK);
            if (!File.Exists(Path_DbFolder)) Directory.CreateDirectory(Path_DbFolder);
            if (!File.Exists(Path_IllustrationsFolder)) Directory.CreateDirectory(Path_IllustrationsFolder);
        }

        internal static void InitDb()
        {
            if (!File.Exists(Path_Db))
                CreateDb();
        }

        private static void CreateDb()
        {
            SqliteConnection dbCo = new SqliteConnection("Data Source = " + Path_Db);
            dbCo.Open();
            foreach (string instruction in TablesCreationString) new SqliteCommand(instruction, dbCo).ExecuteNonQuery();
            dbCo.Close();
        }

    }

}
