using MaGeek.AppData.Entities;
using MaGeek.UI;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Plaziat.CommonWpf;
using ScryfallApi.Client.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace MaGeek.AppBusiness
{

    public static class MageekInitializer
    {

        static string SQL_Translations = @" SELECT cards.name, foreign_data.language, foreign_data.name
                                                 FROM cards JOIN foreign_data ON cards.uuid=foreign_data.uuid
                                                 WHERE 1=1";

        static string SQL_Cards = @" SELECT DISTINCT
                                                    name, 
                                                    type, 
                                                    text,
                                                    keywords,
                                                    power,
                                                    toughness,
                                                    manaCost,
                                                    convertedManaCost,
                                                    colorIdentity,
                                                    faceName
                                                 FROM cards 
                                                 WHERE 1=1";

        static string SQL_Variants = @" SELECT 
                                                    cards.scryfallId,
                                                    cards.rarity,
                                                    cards.artist,
                                                    cards.language,
                                                    sets.name,
                                                    cards.name,
                                                    cards.faceName,
                                                    cards.type
                                                 FROM cards JOIN sets ON cards.setCode=sets.code
                                                 WHERE 1=1";

        public static async Task Initialize()
        {
            App.Events.RaisePreventUIAction(true,"");
            //await ParseCards();
            DateTime modifyTime = File.GetLastWriteTime(App.Config.Path_MtgJsonDownload);
            //if (modifyTime < DateTime.Now.AddMonths(-1))
            {
                App.Events.RaisePreventUIAction(true, "First launch, 1/4 : Downloading data");
                await Task.Delay(100);
                //await DownloadMtgJsonSqlite();
                App.Events.RaisePreventUIAction(true, "First launch, 2/4 : Importing translations");
                await Task.Delay(100);
                await BulkTranslations();
                App.Events.RaisePreventUIAction(true, "First launch, 3/4 : Importing cards");
                await Task.Delay(100);
                //await ParseCards();
                await BulkCards();
                App.Events.RaisePreventUIAction(true, "First launch, 4/4 : Importing variants");
                await BulkVariants();
                App.Events.RaiseUpdateCardCollec();
            }
            App.Events.RaisePreventUIAction(false,"");
        }

        private static async Task DownloadMtgJsonSqlite()
        {
            try
            {
                using var client = new HttpClient();
                using var s = await client.GetStreamAsync("https://mtgjson.com/api/v5/AllPrintings.sqlite");
                using var fs = new FileStream(App.Config.Path_MtgJsonDownload, FileMode.Create);
                await s.CopyToAsync(fs);
            }
            catch (Exception e) { MessageBoxHelper.ShowError("AddDeck", e); }
        }

        private static async Task BulkTranslations()
        {
            await Task.Run(async () => {
                try
                {
                    List<CardTraduction> trads = new();
                    using (var connection = new SqliteConnection("Data Source="+ App.Config.Path_MtgJsonDownload))
                    {
                        await connection.OpenAsync();
                        var command = connection.CreateCommand();
                        command.CommandText = SQL_Translations;
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                trads.Add(new CardTraduction()
                                {
                                    CardId = reader.GetString(0),
                                    Language = reader.GetString(1),
                                    TraductedName= reader.GetString(2),
                                });
                            }
                        }
                    }
                    using var DB = App.DB.GetNewContext();
                    {
                        using var transaction = DB.Database.BeginTransaction();
                        await DB.CardTraductions.AddRangeAsync(trads);
                        await DB.SaveChangesAsync();
                        transaction.Commit();
                    }
                }
                catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }
            });
        }

        private static async Task BulkCards()
        {
            await Task.Run(async () => {
                try
                {
                    // counts for exceptions
                    int ScavengerHuntDiff = 0;
                    int TheSuperlatoriumDiff = 0;
                    int TriviaContestDiff = 0;
                    int IneffableBlessingDiff = 0;
                    int EverythingamajigDiff = 0;
                    int KnightoftheKitchenSinkDiff = 0;
                    int VeryCrypticCommandDiff = 0;
                    int SlySpyDiff = 0;
                    int GarbageElementalDiff = 0;

                    List<MagicCard> cards = new();
                    using (var connection = new SqliteConnection("Data Source=" + App.Config.Path_MtgJsonDownload))
                    {
                        connection.Open();
                        var command = connection.CreateCommand();
                        command.CommandText = SQL_Cards;
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var name = reader.GetString(0);
                                if (!await reader.IsDBNullAsync(9)) name += " ("+reader.GetString(9)+")";

                                var type = reader.GetString(1);
                                string v2tmp = "";
                                if (!await reader.IsDBNullAsync(2)) v2tmp = reader.GetString(2);
                                var Text = v2tmp;
                                string v3tmp = "";
                                if (!await reader.IsDBNullAsync(3)) v3tmp = reader.GetString(3);
                                var KeyWords = v3tmp;
                                string v4tmp = "";
                                if (!await reader.IsDBNullAsync(4)) v4tmp = reader.GetString(4);
                                var Power = v4tmp;
                                string v5tmp = "";
                                if (!await reader.IsDBNullAsync(5)) v5tmp = reader.GetString(5);
                                var Toughness = v5tmp;
                                string v6tmp = "";
                                if (!await reader.IsDBNullAsync(6)) v6tmp = reader.GetString(6);
                                var manacost = v6tmp;
                                var cmc = reader.GetFloat(7);
                                string v8tmp = "";
                                if (!await reader.IsDBNullAsync(8)) v8tmp = reader.GetString(8);
                                var colorId = v8tmp;

                                // Exceptionnal cards

                                // Two different cards with same name...
                                if(name== "Unquenchable Fury") name += " [" + type + "]";
                                // One card dispatched on two
                                if(name== "B.F.M. (Big Furry Monster)") name += manacost == "" ? " [Left]" : " [Right]";
                                // Same Card but different effects(fun)
                                if(name== "Scavenger Hunt") name += " ["+ ScavengerHuntDiff++ + "]";
                                if(name== "The Superlatorium") name += " ["+ TheSuperlatoriumDiff++ + "]";
                                if(name== "Trivia Contest") name += " ["+ TriviaContestDiff++ + "]";
                                if(name== "Ineffable Blessing") name += " ["+ IneffableBlessingDiff++ + "]";
                                if(name== "Everythingamajig") name += " ["+ EverythingamajigDiff++ + "]";
                                if(name== "Knight of the Kitchen Sink") name += " ["+ KnightoftheKitchenSinkDiff++ + "]";
                                if(name== "Very Cryptic Command") name += " ["+ VeryCrypticCommandDiff++ + "]";
                                if(name== "Sly Spy") name += " ["+ SlySpyDiff++ + "]";
                                if(name== "Garbage Elemental") name += " ["+ GarbageElementalDiff++ + "]";
                                
                                cards.Add(new MagicCard(name, type, Text, KeyWords, Power, Toughness, manacost, cmc, colorId));
                            }
                        }
                    }
                    using var DB = App.DB.GetNewContext();
                    {
                        using var transaction = DB.Database.BeginTransaction();
                        await DB.Cards.AddRangeAsync(cards);
                        await DB.SaveChangesAsync();
                        transaction.Commit();
                    }
                }
                catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }

            });
        }

        private static async Task BulkVariants()
        {
            await Task.Run(async () => {
                try
                {
                    // counts for exceptions
                    bool bfmFirst = true;
                    int ScavengerHuntDiff = 0;
                    int TheSuperlatoriumDiff = 0;
                    int TriviaContestDiff = 0;
                    int IneffableBlessingDiff = 0;
                    int EverythingamajigDiff = 0;
                    int KnightoftheKitchenSinkDiff = 0;
                    int VeryCrypticCommandDiff = 0;
                    int SlySpyDiff = 0;
                    int GarbageElementalDiff = 0;

                    List<MagicCardVariant> cards = new();
                    using (var connection = new SqliteConnection("Data Source=" + App.Config.Path_MtgJsonDownload))
                    {
                        connection.Open();
                        var command = connection.CreateCommand();
                        command.CommandText = SQL_Variants;
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var v9 = reader.GetString(0);
                                var v10 = reader.GetString(1);
                                string v11tmp = "";
                                if (!await reader.IsDBNullAsync(2)) v11tmp = reader.GetString(2);
                                var v11 = v11tmp;
                                var v12 = reader.GetString(3);
                                var v13 = reader.GetString(4);
                                var name = reader.GetString(5);
                                if (!await reader.IsDBNullAsync(6)) name += " (" + reader.GetString(6) + ")";

                                // Exceptionnal cards

                                // Two different cards with same name...
                                if (name == "Unquenchable Fury") name += " [" + reader.GetString(7) + "]";
                                // One card dispatched on two
                                if (name == "B.F.M. (Big Furry Monster)")
                                {
                                    if(!bfmFirst)
                                    {
                                        name += " [Left]";
                                        bfmFirst = true;
                                    }
                                    else name += " [Right]";
                                }
                                // Same Card but different effects(fun)
                                if (name == "Scavenger Hunt") name += " [" + ScavengerHuntDiff++ + "]";
                                if (name == "The Superlatorium") name += " [" + TheSuperlatoriumDiff++ + "]";
                                if (name == "Trivia Contest") name += " [" + TriviaContestDiff++ + "]";
                                if (name == "Ineffable Blessing") name += " [" + IneffableBlessingDiff++ + "]";
                                if (name == "Everythingamajig") name += " [" + EverythingamajigDiff++ + "]";
                                if (name == "Knight of the Kitchen Sink") name += " [" + KnightoftheKitchenSinkDiff++ + "]";
                                if (name == "Very Cryptic Command") name += " [" + VeryCrypticCommandDiff++ + "]";
                                if (name == "Sly Spy") name += " [" + SlySpyDiff++ + "]";
                                if (name == "Garbage Elemental") name += " [" + GarbageElementalDiff++ + "]";

                                cards.Add(new MagicCardVariant(v9,v10,v11,v12,v13,name));
                            }
                        }
                    }
                    using var DB = App.DB.GetNewContext();
                    {
                        using var transaction = DB.Database.BeginTransaction();
                        await DB.CardVariants.AddRangeAsync(cards);
                        await DB.SaveChangesAsync();
                        transaction.Commit();
                    }
                }
                catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }

            });
        }

    }

}
