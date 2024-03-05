using PlaziatTools;

namespace MageekCore.Data
{

    public static class Folders
    {

        public static string Roaming { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MaGeekData");


        private static string DbFolder { get; } = Path.Combine(Roaming, "DB");
        public static string File_UpdatePrints { get; } = Path.Combine(DbFolder, "mtgjson.sqlite");
        public static string File_UpdateNewHash { get; } = Path.Combine(DbFolder, "mtgjson.sqlite.sha256");
        public static string File_UpdateOldHash { get; } = Path.Combine(DbFolder, "mtgjson.sqlite.sha256_old");
        public static string File_UpdatePrices { get; } = Path.Combine(DbFolder, "Prices.json");
        public static string File_UpdatePrecos { get; } = Path.Combine(DbFolder, "Precos.zip");
        public static string File_Precos { get; } = Path.Combine(DbFolder, "Precos.json");
        public static string TempPrecoFolder { get; } = Path.Combine(DbFolder, "temp");
        public static string File_CollectionDB { get; } = Path.Combine(DbFolder, "MaGeek.db");
        public static string SetIcon { get; } = Path.Combine(Roaming, "SetIcons");

        public static string Illustrations { get; } = Path.Combine(Roaming, "CardsIllus");
        public static string LayoutFolder { get; } = Path.Combine(Roaming, "Layout");

        public static void InitServerFolders()
        {
            if (!File.Exists(Roaming)) Directory.CreateDirectory(Roaming);
            if (!File.Exists(DbFolder)) Directory.CreateDirectory(DbFolder);
            Logger.Log("Done");
        }
        
        public static void InitializeClientFolders()
        {
            if (!File.Exists(Roaming)) Directory.CreateDirectory(Roaming);
            if (!File.Exists(LayoutFolder)) Directory.CreateDirectory(LayoutFolder);
            if (!File.Exists(Illustrations)) Directory.CreateDirectory(Illustrations);
            if (!File.Exists(SetIcon)) Directory.CreateDirectory(SetIcon);
            if (!File.Exists(PrecosFolder)) Directory.CreateDirectory(PrecosFolder);
            Logger.Log("Done");
        }

    }

}
