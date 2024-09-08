namespace PlaziatTools
{

    /// <summary>
    /// Structure containing usefull data for a local sqlite database
    /// </summary>
    public class SqliteDbInfos
    {

        public string ConnexionString { get; set; }
        public string[] Tables { get; set; }

        public SqliteDbInfos(string path, string[] tables)
        {
            ConnexionString = "Data Source = " + path;
            Tables = tables;
        }

    }

}
