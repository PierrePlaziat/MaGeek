namespace MaCore.Sqlite
{

    public class SqliteDbInfos
    {

        public string ConnexionString { get; set; }
        public string[] Tables { get; set; }

        public SqliteDbInfos(string connexionString, string[] tables)
        {
            ConnexionString = "Data Source = " + connexionString;
            Tables = tables;
        }

    }

}