namespace MageekCore.Tools
{

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