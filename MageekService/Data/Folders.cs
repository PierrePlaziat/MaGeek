using PlaziatTools;

namespace MageekCore.Data
{

    public static class Folders
    {

        public static string Roaming { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MaGeekData");


        private static string DbFolder { get; } = Path.Combine(Roaming, "DB");
        public static string DB { get; } = Path.Combine(DbFolder, "MaGeek.db");
        public static string MtgJson_DB { get; } = Path.Combine(DbFolder, "mtgjson.sqlite");
        public static string MtgJson_NewHash { get; } = Path.Combine(DbFolder, "mtgjson.sqlite.sha256");
        public static string MtgJson_OldHash { get; } = Path.Combine(DbFolder, "mtgjson.sqlite.sha256_old");
        public static string Illustrations { get; } = Path.Combine(Roaming, "CardsIllus");
        public static string SetIcon { get; } = Path.Combine(Roaming, "SetIcons");
        public static string LayoutFolder { get; } = Path.Combine(Roaming, "Layout");
        public static string PrecosFolder { get; } = Path.Combine(Roaming, "Precos");

        public static void InitServerFolders()
        {
            if (!File.Exists(Roaming)) Directory.CreateDirectory(Roaming);
            if (!File.Exists(DbFolder)) Directory.CreateDirectory(DbFolder);
            Logger.Log("Done");
        }
        
        public static void InitClientFolders()
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
