using Microsoft.Data.Sqlite;
using PlaziatTools;

namespace MageekCore.Data.Collection
{

    public class CollectionDbManager
    {

        private string[] description { get; } = new string[] {
            "CREATE TABLE \"CollectedCard\" (\r\n\t\"CardUuid\"\tTEXT,\r\n\t\"Collected\"\tINTEGER\r\n);",
            "CREATE TABLE \"Decks\" (\r\n\t\"DeckId\"\tTEXT,\r\n\t\"Title\"\tTEXT,\r\n\t\"Description\"\tTEXT,\r\n\t\"DeckColors\"\tTEXT,\r\n\t\"CardCount\"\tINTEGER\r\n);",
            "CREATE TABLE \"DeckCard\" (\r\n\t\"DeckId\"\tTEXT,\r\n\t\"CardUuid\"\tTEXT,\r\n\t\"CardType\"\tTEXT,\r\n\t\"Quantity\"\tINTEGER,\r\n\t\"RelationType\"\tINTEGER\r\n);",
            "CREATE TABLE \"FavVariant\" (\r\n\t\"ArchetypeId\"\tTEXT,\r\n\t\"FavUuid\"\tTEXT\r\n);",
            "CREATE TABLE \"Param\" (\r\n\t\"ParamName\"\tTEXT,\r\n\t\"ParamValue\"\tTEXT\r\n);",
            "CREATE TABLE \"Tag\" (\r\n\t\"TagId\"\tTEXT,\r\n\t\"TagContent\"\tTEXT,\r\n\t\"ArchetypeId\"\tTEXT\r\n);"
        };

        public CollectionDbManager()
        {
        }

        public async Task<CollectionDbContext?> GetContext(string user)
        {
            return new CollectionDbContext(GetDBPath(user));
        }

        private string GetDBPath(string user)
        {
            string folder = Path.Combine(PlaziatTools.Paths.Folder_UserSystem, user);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
                CreateDb(folder);
            }
            Logger.Log(folder);
            return Path.Combine(folder, "db.sqlite");
        }

        public void CreateDb(string folder)
        {
            Logger.Log("Data Source = " + Path.Combine(folder, "db.sqlite"));
            using (var dbCo = new SqliteConnection("Data Source = " + Path.Combine(folder, "db.sqlite")))
            {
                dbCo.Open();
                foreach (string instruction in description)
                {
                    using (var command = new SqliteCommand(instruction, dbCo))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

    }

}
