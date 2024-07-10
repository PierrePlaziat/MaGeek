namespace PlaziatTools.Data
{

    public class Folders
    {

        private readonly string appdata;

        public Folders(string appName)
        {
            appdata = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appName);
        }

        public string AppData()
        {
            return appdata; 
        }

    }

}
