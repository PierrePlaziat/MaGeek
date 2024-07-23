namespace PlaziatTools
{

    public static class Paths
    {

        public static string Folder_DesktopInstall { get; } = AppDomain.CurrentDomain.BaseDirectory;
        public static string Folder_ApplicationData { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppDomain.CurrentDomain.FriendlyName);
        public static string Folder_UserSystem { get; } = Path.Combine(Folder_ApplicationData,"Users");
        public static string Folder_Business { get; } = Path.Combine(Folder_ApplicationData, "Business");
        public static string File_UserDb { get; } = Path.Combine(Folder_UserSystem, "Identity.sqlite");

        public static void Init()
        {
            CheckFolder(Folder_DesktopInstall);
            CheckFolder(Folder_ApplicationData);
            CheckFolder(Folder_UserSystem);
            CheckFolder(Folder_Business);
        }

        public static void CheckFolder(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

        public static string GetUserFolder(string user)
        {
            return Path.Combine(Folder_UserSystem, user);
        }
        
    }

}
