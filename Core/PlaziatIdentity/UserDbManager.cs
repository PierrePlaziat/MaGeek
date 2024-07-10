using Microsoft.Data.Sqlite;
using PlaziatTools.Data;

namespace PlaziatIdentity
{

    public class UserDbManager
    {

        private readonly string path;

        private string[] description { get; } = new string[] {
            "CREATE TABLE \"Users\" (\r\n\t\"Id\"\tTEXT,\r\n\t\"Name\"\tTEXT,\r\n\t\"Password\"\tTEXT,\r\n\t\"Mail\"\tTEXT,\r\n\t\"State\"\tINTEGER,\r\n\t\"Type\"\tINTEGER\r\n);"
        };

        public UserDbManager()
        {
            string roaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(roaming, "Identity");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            path = Path.Combine(folder, "Users");
            if (!File.Exists(path)) CreateDb();
        }

        public UserDbContext GetContext()
        {
            return new UserDbContext(path);
        }

        public void CreateDb()
        {
            SqliteConnection dbCo = new SqliteConnection("Data Source = " + path);
            dbCo.Open();
            foreach (string instruction in description) new SqliteCommand(instruction, dbCo).ExecuteNonQuery();
            dbCo.Close();
        }

    }

}
