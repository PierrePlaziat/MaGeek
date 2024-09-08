using Microsoft.Data.Sqlite;
using PlaziatTools;

namespace MageekCore.Data.MtgFetched
{

    public class MtgFetchedDbManager
    {

        private string[] description { get; } = new string[] {
            "CREATE TABLE \"CardArchetypes\" (\r\n\t\"CardUuid\"\tTEXT,\r\n\t\"ArchetypeId\"\tTEXT\r\n);",
            "CREATE TABLE \"CardTraductions\" (\r\n\t\"CardUuid\"\tTEXT,\r\n\t\"Language\"\tTEXT,\r\n\t\"Traduction\"\tTEXT,\r\n\t\"NormalizedTraduction\"\tTEXT\r\n);",
            "CREATE TABLE \"PriceLine\" (\r\n\t\"CardUuid\"\tTEXT,\r\n\t\"PriceEurAccrossTime\"\tTEXT,\r\n\t\"PriceUsdAccrossTime\"\tTEXT,\r\n\t\"LastPriceEur\"\tNUMERIC,\r\n\t\"LastPriceUsd\"\tNUMERIC\r\n);",
        };

        public MtgFetchedDbManager()
        {
        }

        public async Task<MtgFetchedDbContext?> GetContext()
        {
            return new MtgFetchedDbContext(Paths.File_MageekDb);
        }

        public void CreateDb()
        {
            SqliteConnection dbCo = new SqliteConnection("Data Source = " + Paths.File_MageekDb);
            dbCo.Open();
            foreach (string instruction in description) new SqliteCommand(instruction, dbCo).ExecuteNonQuery();
            dbCo.Close();
        }

    }

}
