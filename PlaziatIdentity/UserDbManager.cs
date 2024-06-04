using Microsoft.Data.Sqlite;

namespace PlaziatIdentity
{

    public class UserDbManager
    {

        private readonly string path;

        private string[] description { get; } = new string[] {
            "TODO user table"
        };

        public UserDbManager()
        {
            string roaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(roaming, "Identity");
            path = Path.Combine(folder, "Users"); 
        }

        public UserDbContext GetContext()
        {
            return new UserDbContext(path);
        }

        public void CreateDb(User user)
        {
            SqliteConnection dbCo = new SqliteConnection("Data Source = " + path);
            dbCo.Open();
            foreach (string instruction in description) new SqliteCommand(instruction, dbCo).ExecuteNonQuery();
            dbCo.Close();
        }

    }

}
