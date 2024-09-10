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

        public async Task<CollectionDbContext?> GetContext(string userName)
        {
            if (string.IsNullOrEmpty(userName)) throw new Exception("userName was null or empty");
            string userFolder = Path.Combine(PlaziatTools.Paths.Folder_UserSystem, userName);
            string dbPath = Path.Combine(userFolder, "db.sqlite");
            PlaziatTools.Paths.CheckFolder(userFolder);
            if (!File.Exists(dbPath))
                await CreateDb(dbPath);
            return new CollectionDbContext(dbPath);
        }

        public async Task CreateDb(string dbPath)
        {
            using (var dbCo = new SqliteConnection("Data Source = " + dbPath))
            {
                await dbCo.OpenAsync();
                foreach (string instruction in description)
                {
                    using (var command = new SqliteCommand(instruction, dbCo))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
        }

    }

}
