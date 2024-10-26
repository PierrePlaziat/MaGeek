namespace MageekCore.Data
{

    public static class Paths
    {

        public static string File_MageekDb { get; } = Path.Combine(PlaziatTools.Paths.Folder_Business, "MaGeek.sqlite");
        public static string File_MtgDb { get; } = Path.Combine(PlaziatTools.Paths.Folder_Business, "MtgJson.sqlite");
        public static string File_MtgDb_HashNew { get; } = Path.Combine(PlaziatTools.Paths.Folder_Business, "MtgJson.hash");
        public static string File_MtgDb_HashOld { get; } = Path.Combine(PlaziatTools.Paths.Folder_Business, "MtgJson.hash.old");
        public static string File_PricesJson { get; } = Path.Combine(PlaziatTools.Paths.Folder_Business, "Prices.json");
        public static string File_PrecosZip { get; } = Path.Combine(PlaziatTools.Paths.Folder_Business, "Precos.zip");
        public static string File_PrecosJson { get; } = Path.Combine(PlaziatTools.Paths.Folder_Business, "Precos.json");
        public static string Folder_PrecosTemp { get; } = Path.Combine(PlaziatTools.Paths.Folder_Business, "TempPrecos");
        public static string Folder_SetIcons { get; } = Path.Combine(PlaziatTools.Paths.Folder_Business, "SetIcons");
        public static string Folder_Illustrations { get; } = Path.Combine(PlaziatTools.Paths.Folder_Business, "Illustrations");
        public static string Folder_AvalonLayout { get; } = Path.Combine(PlaziatTools.Paths.Folder_Business, "Layout");
        public static string File_RegCred { get; set; } = Path.Combine(PlaziatTools.Paths.Folder_Business, "RegCreds");

        public static void InitServer()
        {
            PlaziatTools.Paths.CheckFolder(Folder_PrecosTemp);
            PlaziatTools.Paths.CheckFolder(Folder_SetIcons);
        }

        public static void InitClient()
        {
            PlaziatTools.Paths.CheckFolder(Folder_AvalonLayout);
            PlaziatTools.Paths.CheckFolder(Folder_Illustrations);
            PlaziatTools.Paths.CheckFolder(Folder_SetIcons);
        }

    }

}
