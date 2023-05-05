using MaGeek.AppData.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Plaziat.CommonWpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
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
                                                 WHERE availability LIKE '%paper%'";

        static string SQL_Variants = @" SELECT DISTINCT
                                                    cards.scryfallId,
                                                    cards.rarity,
                                                    cards.artist,
                                                    cards.language,
                                                    sets.name,
                                                    cards.name,
                                                    cards.faceName,
                                                    cards.type
                                                 FROM cards JOIN sets ON cards.setCode=sets.code
                                                 WHERE availability LIKE '%paper%'";

        public static async Task FirstLaunch()
        {
            App.Events.RaisePreventUIAction(true,"");
            //var response = MessageBoxHelper.AskUser("It seems to be the first launch, would you like to import all cards? (takes beetween 1~2 hours)");

            //if(response)
            {
                //App.Events.RaisePreventUIAction(true, "First launch, 1/4 : Downloading data");
                //await Task.Delay(100);
                ////await DownloadMtgJsonSqlite();
                //App.Events.RaisePreventUIAction(true, "First launch, 2/4 : Importing translations");
                //await Task.Delay(100);
                //await BulkTranslations();
                //App.Events.RaisePreventUIAction(true, "First launch, 3/4 : Importing cards");
                //await Task.Delay(100);
                ////await ParseCards();
                //await BulkCards();
                App.Events.RaisePreventUIAction(true, "First launch, 4/4 : Importing variants");
                await BulkVariants();
                App.Events.RaiseUpdateCardCollec();
            }
            App.Events.RaisePreventUIAction(false, "");
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
                    // Exceptions gestion
                    bool preventAdd;
                    bool alreadyAdded_ScavengerHunt = false;
                    bool alreadyAdded_TheSuperlatorium = false;
                    bool alreadyAdded_TriviaContest = false;
                    bool alreadyAdded_IneffableBlessing = false;
                    bool alreadyAdded_Everythingamajig = false;
                    bool alreadyAdded_KnightoftheKitchenSink = false;
                    bool alreadyAdded_VeryCrypticCommand = false;
                    bool alreadyAdded_SlySpy = false;
                    bool alreadyAdded_GarbageElemental = false;

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
                                preventAdd = false;

                                var name = reader.GetString(0);

                                //double sided
                                if (!await reader.IsDBNullAsync(9))
                                {
                                    if (!name.StartsWith(reader.GetString(9))) preventAdd = true;
                                }

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
                                if (name== "Unquenchable Fury") name += " [" + type + "]";
                                // One card dispatched on two   
                                if(name== "B.F.M. (Big Furry Monster)") name += manacost == "" ? " [Left]" : " [Right]";
                                
                                // Same Card but different effects(fun)
                                if (name == "Scavenger Hunt")
                                {
                                    if (!alreadyAdded_ScavengerHunt) alreadyAdded_ScavengerHunt = true;
                                    else preventAdd = true;
                                }
                                if (name == "The Superlatorium")
                                {
                                    if (!alreadyAdded_TheSuperlatorium) alreadyAdded_TheSuperlatorium = true;
                                    else preventAdd = true;
                                }
                                if (name == "Trivia Contest")
                                {
                                    if (!alreadyAdded_TriviaContest) alreadyAdded_TriviaContest = true;
                                    else preventAdd = true;
                                }
                                if (name == "Ineffable Blessing")
                                {
                                    if (!alreadyAdded_IneffableBlessing) alreadyAdded_IneffableBlessing = true;
                                    else preventAdd = true;
                                }
                                if (name == "Everythingamajig")
                                {
                                    if (!alreadyAdded_Everythingamajig) alreadyAdded_Everythingamajig = true;
                                    else preventAdd = true;
                                }
                                if (name == "Knight of the Kitchen Sink")
                                {
                                    if (!alreadyAdded_KnightoftheKitchenSink) alreadyAdded_KnightoftheKitchenSink = true;
                                    else preventAdd = true;
                                }
                                if (name == "Very Cryptic Command")
                                {
                                    if (!alreadyAdded_VeryCrypticCommand) alreadyAdded_VeryCrypticCommand = true;
                                    else preventAdd = true;
                                }
                                if (name == "Sly Spy")
                                {
                                    if (!alreadyAdded_SlySpy) alreadyAdded_SlySpy = true;
                                    else preventAdd = true;
                                }
                                if (name == "Garbage Elemental")
                                {
                                    if (!alreadyAdded_GarbageElemental) alreadyAdded_GarbageElemental = true;
                                    else preventAdd = true;
                                }
                                
                                if(!preventAdd) cards.Add(new MagicCard(name, type, Text, KeyWords, Power, Toughness, manacost, cmc, colorId));
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
                DateTime startTime = DateTime.Now;
                try
                {
                    bool preventAdd;
                    using var DB = App.DB.GetNewContext();
                    {
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
                                    preventAdd = false;
                                    var Id = reader.GetString(0);
                                    var Rarity = reader.GetString(1);
                                    string v11tmp = "";
                                    if (!await reader.IsDBNullAsync(2)) v11tmp = reader.GetString(2);
                                    var Artist = v11tmp;
                                    var Lang = reader.GetString(3);
                                    var Set = reader.GetString(4);
                                    var name = reader.GetString(5);

                                    if (!await reader.IsDBNullAsync(6)) name += " (" + reader.GetString(6) + ")";
                                    //double sided
                                    if (!await reader.IsDBNullAsync(6))
                                    {
                                        if (!name.StartsWith(reader.GetString(6))) preventAdd = true;
                                    }

                                    // Two different cards with same name...
                                    if (name == "Unquenchable Fury") name += " [" + reader.GetString(7) + "]";

                                    MagicCard CardRef = await DB.Cards.Where(x => x.CardId == name).FirstOrDefaultAsync();
                                    if (!preventAdd) cards.Add(new MagicCardVariant(Id,Rarity,Artist,Lang,Set, CardRef));
                                }
                            }
                        }
                        using var transaction = DB.Database.BeginTransaction();
                        await DB.CardVariants.AddRangeAsync(cards);
                        await DB.SaveChangesAsync();
                        transaction.Commit();
                    }
                    DateTime endTime = DateTime.Now;
                    MessageBoxHelper.ShowMsg("DONE!!! Took " + (endTime - startTime).TotalMinutes + " mins");
                }
                catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }
            });
        }

    }

}
