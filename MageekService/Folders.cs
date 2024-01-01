namespace MageekService
{
 
    public static class Folders
    {

        public static string Roaming { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MaGeek");
        private static string SDK { get; } = Path.Combine(Roaming, "SDK");

        private static string DbFolder { get; } = Path.Combine(SDK, "DB");
        public static string DB { get; } = Path.Combine(DbFolder, "MaGeek.db");
        public static string MtgJson_DB { get; } = Path.Combine(DbFolder, "mtgjson.sqlite");
        public static string MtgJson_NewHash { get; } = Path.Combine(DbFolder, "mtgjson.sqlite.sha256");
        public static string MtgJson_OldHash { get; } = Path.Combine(DbFolder, "mtgjson.sqlite.sha256_old");
        public static string Illustrations { get; } = Path.Combine(SDK, "CardsIllus");
        public static string SetIcon { get; } = Path.Combine(SDK, "SetIcons");

        public static void InitFolders()
        {
            if (!File.Exists(Roaming)) Directory.CreateDirectory(Roaming);
            if (!File.Exists(SDK)) Directory.CreateDirectory(SDK);
            if (!File.Exists(DbFolder)) Directory.CreateDirectory(DbFolder);
            if (!File.Exists(Illustrations)) Directory.CreateDirectory(Illustrations);
            if (!File.Exists(SetIcon)) Directory.CreateDirectory(SetIcon);
        }

    }

}
