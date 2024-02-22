﻿#pragma warning disable CS8600 // Conversion de littéral ayant une valeur null ou d'une éventuelle valeur null en type non-nullable.
#pragma warning disable CS8602 // Déréférencement d'une éventuelle référence null.

using MageekCore.Data.Collection.Entities;
using MageekCore.Data.Mtg;
using MageekCore.Data.Mtg.Entities;
using MageekCore.Tools;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ScryfallApi.Client.Models;
using System.Text.Json;

namespace MageekCore.Data.Collection
{

    /// <summary>
    /// Mageek database
    /// Call Initialize() first, then you can use GetContext()
    /// to access data through entity framework.
    /// </summary>
    public class CollectionDbManager
    {

        private readonly MtgDbManager mtgDb;
        
        private string[] MageekDbDescription { get; } = new string[] {
            "CREATE TABLE \"CardArchetypes\" (\r\n\t\"CardUuid\"\tTEXT,\r\n\t\"ArchetypeId\"\tTEXT\r\n);",
            "CREATE TABLE \"CardTraductions\" (\r\n\t\"CardUuid\"\tTEXT,\r\n\t\"Language\"\tTEXT,\r\n\t\"Traduction\"\tTEXT,\r\n\t\"NormalizedTraduction\"\tTEXT\r\n);",
            "CREATE TABLE \"CollectedCard\" (\r\n\t\"CardUuid\"\tTEXT,\r\n\t\"Collected\"\tINTEGER\r\n);",
            "CREATE TABLE \"Decks\" (\r\n\t\"DeckId\"\tTEXT,\r\n\t\"Title\"\tTEXT,\r\n\t\"Description\"\tTEXT,\r\n\t\"DeckColors\"\tTEXT,\r\n\t\"CardCount\"\tINTEGER\r\n);",
            "CREATE TABLE \"DeckCard\" (\r\n\t\"DeckId\"\tTEXT,\r\n\t\"CardUuid\"\tTEXT,\r\n\t\"CardType\"\tTEXT,\r\n\t\"Quantity\"\tINTEGER,\r\n\t\"RelationType\"\tINTEGER\r\n);",
            "CREATE TABLE \"FavVariant\" (\r\n\t\"ArchetypeId\"\tTEXT,\r\n\t\"FavUuid\"\tTEXT\r\n);",
            "CREATE TABLE \"Param\" (\r\n\t\"ParamName\"\tTEXT,\r\n\t\"ParamValue\"\tTEXT\r\n);",
            "CREATE TABLE \"PriceLine\" (\r\n\t\"CardUuid\"\tTEXT,\r\n\t\"LastUpdate\"\tTEXT,\r\n\t\"PriceEur\"\tTEXT,\r\n\t\"PriceUsd\"\tTEXT,\r\n\t\"EdhrecScore\"\tINTEGER\r\n);",
            "CREATE TABLE \"Tag\" (\r\n\t\"TagId\"\tTEXT,\r\n\t\"TagContent\"\tTEXT,\r\n\t\"ArchetypeId\"\tTEXT\r\n);"
        };

        public CollectionDbManager(
            MtgDbManager mtgDb
        ){
            this.mtgDb = mtgDb;
        }

        public async Task<CollectionDbContext?> GetContext()
        {
            await Task.Delay(0);
            return new CollectionDbContext(Folders.DB);
        }

        public void CreateDb()
        {
            SqliteConnection dbCo = new SqliteConnection("Data Source = " + Folders.DB);
            dbCo.Open();
            foreach (string instruction in MageekDbDescription) new SqliteCommand(instruction, dbCo).ExecuteNonQuery();
            dbCo.Close();
        }

        public async Task FetchMtgData()
        {
            try
            {
                Logger.Log("Start");
                List<Task> tasks = new()
                {
                    GatherArchetypes(),
                    GatherTranslations(),
                    RetrieveSetIcons()
                };
                await Task.WhenAll(tasks);
                Logger.Log("Done");
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }

        public async Task GatherArchetypes()
        {
            
            try
            {
                List<ArchetypeCard> archetypes = new();
                Logger.Log("Parsing...");
                using (MtgDbContext mtgSqliveContext = await mtgDb.GetContext())
                {
                    await Task.Run(() => {
                        foreach (Cards card in mtgSqliveContext.cards)
                        {
                            //if (!(archetypes.Any(x => x.CardUuid == card.Uuid))) // todo remove later (duplicated uuid bug)
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
                    });
                }
                Logger.Log("Saving...");
                using (CollectionDbContext collectionDbContext = await GetContext())
                {
                    await collectionDbContext.CardArchetypes.ExecuteDeleteAsync();
                    using var transaction = await collectionDbContext.Database.BeginTransactionAsync();
                    await collectionDbContext.CardArchetypes.AddRangeAsync(archetypes);
                    await collectionDbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                Logger.Log("Done");
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }

        public async Task GatherTranslations()
        {
            try
            {
                List<CardTraduction> traductions = new();
                Logger.Log("Parsing...");
                using (MtgDbContext mtgSqliveContext = await mtgDb.GetContext())
                {
                    foreach (CardForeignData traduction in mtgSqliveContext.cardForeignData)
                    {
                        await Task.Run(() => {
                            if (traduction.FaceName == null)
                            {
                                traductions.Add(
                                    new CardTraduction()
                                    {
                                        CardUuid = traduction.Uuid,
                                        Language = traduction.Language,
                                        Traduction = traduction.Name,
                                        NormalizedTraduction = traduction.Language != "Korean" && traduction.Language != "Arabic"
                                            ? traduction.Name.RemoveDiacritics().Replace('-', ' ').ToLower()
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
                        });
                    }
                }
                Logger.Log("Saving...");
                using (CollectionDbContext collectionDbContext = await GetContext())
                {
                    await collectionDbContext.CardTraductions.ExecuteDeleteAsync();
                    using var transaction = await collectionDbContext.Database.BeginTransactionAsync();
                    await collectionDbContext.CardTraductions.AddRangeAsync(traductions);
                    await collectionDbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                Logger.Log("Done");
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }

        public async Task RetrieveSetIcons()
        {
            Logger.Log("Start");
            try
            {
                string json_data = await HttpUtils.Get("https://api.scryfall.com/sets/");
                var Setz = JsonSerializer.Deserialize<ResultList<Set>>(json_data);
                using (HttpClient client = new HttpClient())
                {
                    foreach (Set set in Setz.Data)
                    {
                        var uri = set.IconSvgUri;
                        string localFileName = Path.Combine(Folders.SetIcon, set.Code.ToUpper() + "_.svg");
                        if (!File.Exists(localFileName))
                        {
                            using (var s = await client.GetStreamAsync(uri))
                            {
                                try
                                {
                                    using (var fs = new FileStream(localFileName, FileMode.OpenOrCreate))
                                    {
                                        await s.CopyToAsync(fs);
                                    }
                                }
                                catch (Exception e) 
                                {
                                    Logger.Log(uri.ToString());
                                    Logger.Log(localFileName);
                                    Logger.Log(e.ToString());
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e) { Logger.Log(e); }
            Logger.Log("Done");
        }

    }

}
