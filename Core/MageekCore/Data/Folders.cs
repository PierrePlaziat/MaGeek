using PlaziatCore;

namespace MageekCore.Data
{

    public static class Folders
    {

        // Both sides

        public static string Roaming { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MaGeek");

        public static void CheckFolder(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

        // Server side

        public static string UserData { get; } = Path.Combine(Roaming, "UserData");
        public static string UserDbPath { get; } = Path.Combine(UserData, "Users");
        private static string CommonData { get; } = Path.Combine(Roaming, "CommonData");
        public static string File_UpdatePrints { get; } = Path.Combine(CommonData, "mtgjson.sqlite");
        public static string File_UpdateNewHash { get; } = Path.Combine(CommonData, "mtgjson.sqlite.sha256");
        public static string File_UpdateOldHash { get; } = Path.Combine(CommonData, "mtgjson.sqlite.sha256_old");
        public static string File_UpdatePrices { get; } = Path.Combine(CommonData, "Prices.json");
        public static string File_UpdatePrecos { get; } = Path.Combine(CommonData, "Precos.zip");
        public static string File_Precos { get; } = Path.Combine(CommonData, "Precos.json");
        public static string File_CollectionDB { get; } = Path.Combine(CommonData, "MaGeek.db");
        public static string TempPrecoFolder { get; } = Path.Combine(CommonData, "temp");

        public static void InitServerFolders()
        {
            Logger.Log("...");
            CheckFolder(Roaming);
            CheckFolder(UserData);
            CheckFolder(CommonData);
            CheckFolder(TempPrecoFolder);
            Logger.Log("Done");
        }

        // Client side

        public static string Illustrations { get; } = Path.Combine(Roaming, "CardsIllus");
        public static string LayoutFolder { get; } = Path.Combine(Roaming, "Layout");
        public static string SetIcon { get; } = Path.Combine(Roaming, "SetIcons");

        public static void InitializeClientFolders()
        {
            Logger.Log("...");
            CheckFolder(Roaming);
            CheckFolder(LayoutFolder);
            CheckFolder(Illustrations);
            CheckFolder(SetIcon);
            Logger.Log("Done");
        }

    }

}
