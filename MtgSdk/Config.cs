using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MageekSdk
{
    public class Config
    {

        public static string Url_MtgjsonHash { get; } = "https://mtgjson.com/api/v5/AllPrintings.sqlite.sha256";
        public static string Url_MtgjsonData { get; } = "https://mtgjson.com/api/v5/AllPrintings.sqlite";
        public static string Path_RoamingFolder { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MaGeek");
        public static string Path_DbFolder { get; } = Path.Combine(Path_RoamingFolder, "DB");
        public static string Path_Db { get; } = Path.Combine(Path_DbFolder, "MaGeek.db");
        public static string Path_MtgJsonDownload { get; } = Path.Combine(Path_DbFolder, "mtgjson.sqlite");
        public static string Path_MtgJsonDownload_NewHash { get; } = Path.Combine(Path_DbFolder, "mtgjson.sqlite.sha256");
        public static string Path_MtgJsonDownload_OldHash { get; } = Path.Combine(Path_DbFolder, "mtgjson.sqlite.sha256_old");
        public static string Path_IllustrationsFolder { get; } = Path.Combine(Path_RoamingFolder, "CardsIllus");

        #region Debug
        // Here just in case,
        // can be usefull when the endpoint is down for example,
        // Make sure to reset to false after usage.
        public static bool bypass_Hash = false;
        public static bool bypass_Data = false;
        #endregion

        public static string[] TablesCreationString { get; } = new string[] {
            "CREATE TABLE \"CardModels\" (\r\n\t\"CardId\"\tTEXT,\r\n\t\"Type\"\tTEXT,\r\n\t\"ManaCost\"\tREAL,\r\n\t\"Cmc\"\tNUMERIC,\r\n\t\"ColorIdentity\"\tTEXT,\r\n\t\"DevotionB\"\tINTEGER,\r\n\t\"DevotionW\"\tINTEGER,\r\n\t\"DevotionU\"\tINTEGER,\r\n\t\"DevotionR\"\tINTEGER,\r\n\t\"DevotionG\"\tINTEGER,\r\n\t\"Text\"\tTEXT,\r\n\t\"Keywords\"\tTEXT,\r\n\t\"Power\"\tTEXT,\r\n\t\"Toughness\"\tTEXT,\r\n\t\"FavouriteVariant\"\tTEXT,\r\n\t\"Got\"\tINTEGER\r\n);",
            "CREATE TABLE \"CardTags\" (\r\n\t\"Id\"\tINTEGER,\r\n\t\"Tag\"\tTEXT,\r\n\t\"CardId\"\tINTEGER,\r\n\tPRIMARY KEY(\"Id\")\r\n);",
            "CREATE TABLE \"CardTraductions\" (\r\n\t\"TraductionId\"\tINTEGER,\r\n\t\"CardId\"\tTEXT,\r\n\t\"Language\"\tTEXT,\r\n\t\"TraductedName\"\tTEXT,\r\n\t\"Normalized\"\tTEXT,\r\n\tPRIMARY KEY(\"TraductionId\")\r\n);",
            "CREATE TABLE \"DeckCards\" (\r\n\t\"DeckId\"\tINTEGER,\r\n\t\"CardId\"\tTEXT,\r\n\t\"Quantity\"\tINTEGER,\r\n\t\"RelationType\"\tINTEGER,\r\n\tPRIMARY KEY(\"CardId\",\"DeckId\")\r\n);",
            "CREATE TABLE \"Decks\" (\r\n\t\"DeckId\"\tINTEGER,\r\n\t\"Title\"\tTEXT,\r\n\t\"Description\"\tTEXT,\r\n\t\"DeckColors\"\tTEXT,\r\n\t\"CardCount\"\tINTEGER,\r\n\tPRIMARY KEY(\"DeckId\")\r\n);",
            "CREATE TABLE \"Params\" (\r\n\t\"ParamName\"\tTEXT,\r\n\t\"ParamValue\"\tTEXT\r\n);",
            "CREATE TABLE \"User_FavCards\" (\r\n\t\"CardModelId\"\tTEXT,\r\n\t\"CardVariantId\"\tTEXT,\r\n\tPRIMARY KEY(\"CardModelId\")\r\n);",
            "CREATE TABLE \"User_GotCards\" (\r\n\t\"CardVariantId\"\tTEXT,\r\n\t\"CardModelId\"\tTEXT,\r\n\t\"Got\"\tINTEGER,\r\n\tPRIMARY KEY(\"CardVariantId\")\r\n);",
        };


    }
}
