using MaGeek.Entities;
using MaGeek.Framework;
using MaGeek.Framework.Extensions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MaGeek.AppBusiness
{

    /// <summary>
    /// Interracts with MtgJson, called at first launch
    /// TODO : currently no way to import only missing data later
    /// </summary>
    public static class MageekBulkinator
    {

        #region SQL

        private const string SQL_AllCardTraductions = @"
            SELECT 
                cards.name, foreign_data.language, foreign_data.name
            FROM 
                cards JOIN foreign_data ON cards.uuid=foreign_data.uuid
            WHERE 
                1=1";

        private const string SQL_AllCardModels = @"
            SELECT DISTINCT
                name, type, text, keywords, power, toughness, manaCost, convertedManaCost, colorIdentity, faceName
            FROM 
                cards 
            WHERE 
                availability LIKE '%paper%'";

        private const string SQL_AllCardVariants = @" 
            SELECT DISTINCT
                cards.scryfallId, cards.rarity, cards.artist, cards.language, sets.name, cards.name, cards.faceName, cards.type
            FROM 
                cards JOIN sets ON cards.setCode=sets.code
            WHERE 
                availability LIKE '%paper%'";

        #endregion

        #region Before Launch

        public static async Task Download_MtgJsonSqlite()
        {
            try
            {
                using var client = new HttpClient();
                using var s = await client.GetStreamAsync("https://mtgjson.com/api/v5/AllPrintings.sqlite");
                using var fs = new FileStream(App.Config.Path_MtgJsonDownload, FileMode.Create);
                await s.CopyToAsync(fs);
            }
            catch (Exception e) { Log.Write(e); }
        }

        public static async Task Bulk_CardTraductions()
        {
            await Task.Run((Func<Task>)(async () => {
                try
                {
                    List<CardTraduction> trads = new();
                    using (var connection = new SqliteConnection("Data Source=" + App.Config.Path_MtgJsonDownload))
                    {
                        await connection.OpenAsync();
                        var command = connection.CreateCommand();
                        command.CommandText = SQL_AllCardTraductions;
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            string id = "";
                            string lang = "";
                            string traducted = "";
                            string normalized = "";
                            while (await reader.ReadAsync())
                            {
                                try
                                {
                                    id = reader.GetString(0);
                                    lang = reader.GetString(1);
                                    traducted = reader.GetString(2);
                                    if(lang != "Korean" && lang != "Arabic")
                                    {
                                        normalized = StringExtension.RemoveDiacritics(traducted).Replace('-',' ').ToLower();
                                    }
                                    else normalized = traducted;
                                    trads.Add(new CardTraduction()
                                    {
                                        CardId = id,
                                        Language = lang,
                                        TraductedName = traducted,
                                        Normalized = normalized,
                                    });
                                }
                                catch
                                {
                                }
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
                catch (Exception e) { Log.Write(e); }
            }));
        }

        #endregion

        #region Once Launched

        public static async Task ReBulk_CardTraductions()
        {
            Log.Write("Reimporting traductions");
            App.Events.RaisePreventUIAction(true, "Reimporting traductions");
            using (var DB = App.DB.GetNewContext())
            {
                await DB.CardTraductions.ExecuteDeleteAsync();
            }
            Log.Write("> Deleted Old Data");
            DateTime start = DateTime.Now;
            await Bulk_CardTraductions();
            DateTime end = DateTime.Now;
            App.Events.RaisePreventUIAction(false, "");
            Log.Write("Traductions reimporter, took " + (end-start).TotalMinutes + " min.");
        }

        public static async Task Bulk_Cards(bool includeFun)
        {
            App.Events.RaisePreventUIAction(true, "Importing Cards, takes few minutes.");
            await Bulk_CardModels(includeFun);
            await Bulk_CardVariants(includeFun);
            App.Events.RaiseUpdateCardCollec();
            App.Events.RaisePreventUIAction(false, "");
        }
        private static async Task Bulk_CardModels(bool includeFun)
        {
            await Task.Run((Func<Task>)(async () => {
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

                    List<CardModel> cards = new();
                    using (var connection = new SqliteConnection("Data Source=" + App.Config.Path_MtgJsonDownload))
                    {
                        connection.Open();
                        var command = connection.CreateCommand();
                        command.CommandText = SQL_AllCardModels;
                        if (includeFun) command.CommandText += " AND isFunny=0";
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                preventAdd = false;

                                var name = reader.GetString(0);


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

                                //double sided
                                if (!await reader.IsDBNullAsync(9))
                                {
                                    if (!name.StartsWith(reader.GetString(9)))
                                    {
                                        preventAdd = true;
                                    }
                                }

                                // Exceptionnal cards
                                // Two different cards with same name...
                                if (name == "Unquenchable Fury") name += " [" + type + "]";
                                // One card dispatched on two   
                                if(name == "B.F.M. (Big Furry Monster)") name += manacost == "" ? " [Left]" : " [Right]";
                                
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
                                
                                if(!preventAdd) cards.Add(new CardModel(name, type, Text, KeyWords, Power, Toughness, manacost, cmc, colorId));
                            }
                        }
                    }
                    using var DB = App.DB.GetNewContext();
                    {
                        using var transaction = DB.Database.BeginTransaction();
                        await DB.CardModels.AddRangeAsync(cards);
                        await DB.SaveChangesAsync();
                        transaction.Commit();
                    }
                }
                catch (Exception e) { Log.Write(e); }
            }));
        }
        private static async Task Bulk_CardVariants(bool includeFun)
        {
            await Task.Run((Func<Task>)(async () => {
                DateTime startTime = DateTime.Now;
                try
                {
                    bool preventAdd;
                    using var DB = App.DB.GetNewContext();
                    {
                        List<CardVariant> cards = new();
                        using (var connection = new SqliteConnection("Data Source=" + App.Config.Path_MtgJsonDownload))
                        {
                            connection.Open();
                            var command = connection.CreateCommand();
                            command.CommandText = SQL_AllCardVariants;
                            if (includeFun) command.CommandText += " AND isFunny=0";
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

                                    //double sided
                                    if (!await reader.IsDBNullAsync(6))
                                    {
                                        if (!name.StartsWith(reader.GetString(6)))
                                        {
                                            preventAdd = true;
                                        }
                                    }

                                    // Two different cards with same name...
                                    if (name == "Unquenchable Fury") name += " [" + reader.GetString(7) + "]";

                                    CardModel CardRef = await DB.CardModels.Where(x => x.CardId == name).FirstOrDefaultAsync();
                                    if (!preventAdd) cards.Add(new CardVariant(Id, Rarity, Artist, Lang, Set, CardRef));
                                }
                            }
                        }
                        using var transaction = DB.Database.BeginTransaction();
                        await DB.CardVariants.AddRangeAsync(cards);
                        await DB.SaveChangesAsync();
                        transaction.Commit();
                    }
                    DateTime endTime = DateTime.Now;
                    //MessageBoxHelper.ShowMsg("DONE!!! Took " + (endTime - startTime).TotalMinutes + " mins");
                }
                catch (Exception e) { Log.Write(e); }
            }));
        }

        #endregion

    }

}
