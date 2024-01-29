#pragma warning disable CS8600 // Conversion de littéral ayant une valeur null ou d'une éventuelle valeur null en type non-nullable.
#pragma warning disable CS8602 // Déréférencement d'une éventuelle référence null.

using MageekService.Data.Collection.Entities;
using MageekService.Data.Mtg;
using MageekService.Data.Mtg.Entities;
using MageekService.Tools;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScryfallApi.Client.Models;
using System.Net;
using System.Text.Json;

namespace MageekService.Data.Collection
{

    /// <summary>
    /// Mageek database
    /// Call Initialize() first, then you can use GetContext()
    /// to access data through entity framework.
    /// </summary>
    public class CollectionDbManager
    {

        private readonly ILogger<CollectionDbManager> logger;

        public CollectionDbManager(ILogger<CollectionDbManager> logger)
        {
            this.logger = logger;
        }

        public async Task<CollectionDbContext?> GetContext()
        {
            await Task.Delay(0);
            return new CollectionDbContext(Folders.DB);
        }

        private string[] MageekDbDescription { get; } = new string[] {
            "CREATE TABLE \"CardArchetypes\" (\r\n\t\"CardUuid\"\tTEXT,\r\n\t\"ArchetypeId\"\tTEXT\r\n);",
            "CREATE TABLE \"CardTraductions\" (\r\n\t\"CardUuid\"\tTEXT,\r\n\t\"Language\"\tTEXT,\r\n\t\"Traduction\"\tTEXT,\r\n\t\"NormalizedTraduction\"\tTEXT\r\n);",
            "CREATE TABLE \"CollectedCard\" (\r\n\t\"CardUuid\"\tTEXT,\r\n\t\"Collected\"\tINTEGER\r\n);",
            "CREATE TABLE \"Decks\" (\r\n\t\"DeckId\"\tTEXT,\r\n\t\"Title\"\tTEXT,\r\n\t\"Description\"\tTEXT,\r\n\t\"DeckColors\"\tTEXT,\r\n\t\"CardCount\"\tINTEGER\r\n);",
            "CREATE TABLE \"DeckCard\" (\r\n\t\"DeckId\"\tTEXT,\r\n\t\"CardUuid\"\tTEXT,\r\n\t\"Quantity\"\tINTEGER,\r\n\t\"RelationType\"\tINTEGER\r\n);",
            "CREATE TABLE \"FavVariant\" (\r\n\t\"ArchetypeId\"\tTEXT,\r\n\t\"FavUuid\"\tTEXT\r\n);",
            "CREATE TABLE \"Param\" (\r\n\t\"ParamName\"\tTEXT,\r\n\t\"ParamValue\"\tTEXT\r\n);",
            "CREATE TABLE \"PriceLine\" (\r\n\t\"CardUuid\"\tTEXT,\r\n\t\"LastUpdate\"\tTEXT,\r\n\t\"PriceEur\"\tTEXT,\r\n\t\"PriceUsd\"\tTEXT,\r\n\t\"EdhrecScore\"\tINTEGER\r\n);",
            "CREATE TABLE \"Tag\" (\r\n\t\"TagId\"\tTEXT,\r\n\t\"TagContent\"\tTEXT,\r\n\t\"ArchetypeId\"\tTEXT\r\n);"
        };

        public void CreateDb()
        {
            SqliteConnection dbCo = new SqliteConnection("Data Source = " + Folders.DB);
            dbCo.Open();
            foreach (string instruction in MageekDbDescription) new SqliteCommand(instruction, dbCo).ExecuteNonQuery();
            dbCo.Close();
        }

        public async Task FetchMtg()
        {
            try
            {
                logger.LogInformation("Start");
                List<Task> tasks = new()
                {
                    FetchMtg_Archetypes(),
                    FetchMtg_Traductions(),
                    FetchMtg_SetIcons()
                };
                await Task.WhenAll(tasks);
                logger.LogInformation("Done");
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
        }

        public async Task FetchMtg_Archetypes()
        {
            try
            {
                List<ArchetypeCard> archetypes = new();
                logger.LogInformation("Parsing...");
                using (MtgDbContext mtgSqliveContext = await MtgDbManager.GetContext())
                {

                    foreach (Cards card in mtgSqliveContext.cards)
                    {
                        if (!(archetypes.Any(x => x.CardUuid == card.Uuid))) // todo remove later (duplicated uuid bug)
                        {
                            archetypes.Add(
                                new ArchetypeCard()
                                {
                                    ArchetypeId = card.Name != null ? card.Name : string.Empty,
                                    CardUuid = card.Uuid
                                }
                            );
                        }
                    }
                }
                logger.LogInformation("Saving...");
                using (CollectionDbContext collectionDbContext = await GetContext())
                {
                    await collectionDbContext.CardArchetypes.ExecuteDeleteAsync();
                    using var transaction = collectionDbContext.Database.BeginTransaction();
                    await collectionDbContext.CardArchetypes.AddRangeAsync(archetypes);
                    await collectionDbContext.SaveChangesAsync();
                    transaction.Commit();
                }
                logger.LogInformation("Done");
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
        }

        public async Task FetchMtg_Traductions()
        {
            try
            {
                List<CardTraduction> traductions = new();
                logger.LogInformation("Parsing...");
                using (MtgDbContext mtgSqliveContext = await MtgDbManager.GetContext())
                {
                    foreach (CardForeignData traduction in mtgSqliveContext.cardForeignData)
                    {
                        if (traduction.FaceName == null)
                        {
                            traductions.Add(
                                new CardTraduction()
                                {
                                    CardUuid = traduction.Uuid,
                                    Language = traduction.Language,
                                    Traduction = traduction.Name,
                                    NormalizedTraduction = traduction.Language != "Korean" && traduction.Language != "Arabic"
                                        ? StringExtension.RemoveDiacritics(traduction.Name).Replace('-', ' ').ToLower()
                                        : traduction.Name
                                }
                            );
                        }
                        else
                        {
                            traductions.Add(
                                new CardTraduction()
                                {
                                    CardUuid = traduction.Uuid,
                                    Language = traduction.Language,
                                    Traduction = traduction.FaceName,
                                    NormalizedTraduction = traduction.Language != "Korean" && traduction.Language != "Arabic"
                                        ? StringExtension.RemoveDiacritics(traduction.FaceName).Replace('-', ' ').ToLower()
                                        : traduction.FaceName
                                }
                            );
                        }
                    }
                }
                logger.LogInformation("Saving...");
                using (CollectionDbContext collectionDbContext = await GetContext())
                {
                    await collectionDbContext.CardTraductions.ExecuteDeleteAsync();
                    using var transaction = collectionDbContext.Database.BeginTransaction();
                    await collectionDbContext.CardTraductions.AddRangeAsync(traductions);
                    await collectionDbContext.SaveChangesAsync();
                    transaction.Commit();
                }
                logger.LogInformation("Done");
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
        }

        public async Task FetchMtg_SetIcons()
        {
            logger.LogInformation("Start");
            try
            {
                string json_data = await HttpUtils.Get("https://api.scryfall.com/sets/");
                var Setz = JsonSerializer.Deserialize<ResultList<Set>>(json_data);
                foreach (Set set in Setz.Data)
                {
                    try
                    {
                        var uri = set.IconSvgUri;
                        string localFileName = Path.Combine(Folders.SetIcon, set.Code.ToUpper() + ".svg");
                        if (!File.Exists(localFileName))
                        {
                            using (WebClient client = new WebClient())
                            {
                                await client.DownloadFileTaskAsync(uri, localFileName);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Log(e);
                    }
                }
            }
            catch (Exception e) 
            { 
                Logger.Log(e); 
            }
            Logger.Log("Done");
        }

    }
}
