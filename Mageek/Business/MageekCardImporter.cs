using MaGeek.Entities;
using MaGeek.Framework.Extensions;
using MaGeek.Framework.Utils;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ScryfallApi.Client.Apis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MaGeek.AppBusiness
{

    /// <summary>
    /// Bulk data import gestion using MtgJson's sqlite
    /// </summary>
    public static class MageekCardImporter
    {

        #region SQL

        private const string SQL_AllSets = @"
            SELECT 
                name, type, block, baseSetSize, totalSetSize, releaseDate
            FROM 
                sets
            WHERE 
                1=1";
        
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

        #region Public Methods

        public static async Task<bool> AreDataOutdated()
        {
            Log.Write("Checking cards update...");
            try
            {
                bool? tooOld = FileUtils.IsFileOlder(App.Config.Path_MtgJsonDownload_OldHash, new TimeSpan(0, 23, 0, 0));
                // Dont check too often
                if (tooOld.HasValue && !tooOld.Value)
                {
                    Log.Write("> No Update, done.");
                    return false;
                }
                await DownloadHash();
                Log.Write("> Hash Check");
                // Hash Check
                bool check = true;
                if (File.Exists(App.Config.Path_MtgJsonDownload_OldHash))
                {
                    check = FileUtils.FileContentDiffers(
                        App.Config.Path_MtgJsonDownload_NewHash,
                        App.Config.Path_MtgJsonDownload_OldHash
                    );
                }
                if(check) Log.Write("Cards update available");
                else Log.Write("No cards update available");
                return check;
            }
            catch (Exception e)
            {
                Log.Write(e, "AreDataOutdated");
                return false;
            }
        }

        public static async Task DownloadHash()
        {
            Log.Write("> Hash Download...");
            // Hash Download
            using (var client = new HttpClient())
            {
                using (var hash = await client.GetStreamAsync("https://mtgjson.com/api/v5/AllPrintings.sqlite.sha256"))
                using (var fs_NewHash = new FileStream(App.Config.Path_MtgJsonDownload_NewHash, FileMode.Create))
                {
                    await hash.CopyToAsync(fs_NewHash);
                }
            }
        }

        public async static Task DownloadBulkData()
        {
            Log.Write("Downloading bulk Data...");
            try
            {
                // File Download
                using (var client = new HttpClient())
                using (var mtgjson_sqlite = await client.GetStreamAsync("https://mtgjson.com/api/v5/AllPrintings.sqlite"))
                {
                    using var fs_mtgjson_sqlite = new FileStream(App.Config.Path_MtgJsonDownload, FileMode.Create);
                    await mtgjson_sqlite.CopyToAsync(fs_mtgjson_sqlite);
                }
                // Save Hash
                File.Copy(App.Config.Path_MtgJsonDownload_NewHash, App.Config.Path_MtgJsonDownload_OldHash);
            }
            catch (Exception e) { Log.Write(e, "DownloadBulkData : "); }
            Log.Write("Done");
        }

        public static async Task ImportAllData(bool isFirstOne, bool includeFun = false)
        {
            Log.Write("Importing all data...");
            try
            {
                Log.Write("ImportAllData : Bulk_CardTraductions...");
                await Bulk_CardTraductions();
                Log.Write("ImportAllData : Bulk_Sets...");
                await Bulk_Sets();
                Log.Write("ImportAllData : Bulk_CardModels...");
                await Bulk_CardModels(includeFun);
                Log.Write("ImportAllData : Bulk_CardVariants...");
                await Bulk_CardVariants(isFirstOne, includeFun);
                Log.Write("Done");
            }
            catch (Exception e) { Log.Write(e, "ImportAllData"); }
        }

        #endregion

        #region Import Gestion

        private static async Task Bulk_CardTraductions()
        {
            // Delete old data
            using (var DB = App.DB.NewContext) await DB.CardTraductions.ExecuteDeleteAsync();
            // Insert New Data
            await Task.Run(async () => {
                List<CardTraduction> trads = new();
                try
                {
                    // Connect
                    using (var MtgJsonSqliteConnexion = new SqliteConnection("Data Source=" + App.Config.Path_MtgJsonDownload))
                    {
                        await MtgJsonSqliteConnexion.OpenAsync();
                        var command = MtgJsonSqliteConnexion.CreateCommand();
                        command.CommandText = SQL_AllCardTraductions;
                        // Process
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
                                    if (lang != "Korean" && lang != "Arabic")
                                    {
                                        normalized = StringExtension.RemoveDiacritics(traducted).Replace('-', ' ').ToLower();
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
                                catch (Exception e) { Log.Write(e, "Bulk_CardTraductions"); }
                            }
                        }
                    }
                    // Save
                    using (var DB = App.DB.NewContext)
                    {
                        using var transaction = DB.Database.BeginTransaction();
                        await DB.CardTraductions.AddRangeAsync(trads);
                        await DB.SaveChangesAsync();
                        transaction.Commit();
                    }
                }
                catch (Exception e) { Log.Write(e, "Bulk_CardTraductions"); }
            });
        }

        private static async Task Bulk_Sets()
        {
            // Delete old data
            using (var DB = App.DB.NewContext) await DB.Sets.ExecuteDeleteAsync();
            // Insert New Data
            await Task.Run(async () =>
            {
                List<MtgSet> sets = new();
                try
                {
                    // Connect
                    using (var MtgJsonSqliteConnexion = new SqliteConnection("Data Source=" + App.Config.Path_MtgJsonDownload))
                    {
                        await MtgJsonSqliteConnexion.OpenAsync();
                        var command = MtgJsonSqliteConnexion.CreateCommand();
                        command.CommandText = SQL_AllSets;
                        // Process
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            string name;
                            string type;
                            string block;
                            int baseSetSize;
                            int totalSetSize;
                            DateOnly releaseDate;
                            while (await reader.ReadAsync())
                            {
                                name = "";
                                type = "";
                                block = "";
                                baseSetSize = 0;
                                totalSetSize = 0;
                                releaseDate = DateOnly.MinValue;
                                try
                                {
                                    name = reader.GetString(0);
                                    type = reader.GetString(1);
                                    block = "";
                                    if (!reader.IsDBNull(2)) block = reader.GetString(2);
                                    baseSetSize = int.Parse(reader.GetString(3));
                                    totalSetSize = int.Parse(reader.GetString(4));
                                    sets.Add(new MtgSet()
                                    {
                                        Name = name,
                                        Type = type,
                                        Block = block,
                                        BaseSetSize = baseSetSize,
                                        TotalSetSize = totalSetSize,
                                        ReleaseDate = releaseDate,
                                    });
                                }
                                catch (Exception e) { Log.Write(e, "Bulk_Sets"); }
                            }
                        }
                    }
                    // Save
                    using (var DB = App.DB.NewContext)
                    {
                        using var transaction = DB.Database.BeginTransaction();
                        await DB.Sets.AddRangeAsync(sets);
                        await DB.SaveChangesAsync();
                        transaction.Commit();
                    }
                }
                catch (Exception e) { Log.Write(e, "Bulk_Sets"); }
            });
        }
    
        private static async Task Bulk_CardModels(bool includeFun)
        {
            // Delete old data
            using (var DB = App.DB.NewContext) await DB.CardModels.ExecuteDeleteAsync();
            // Insert New Data
            await Task.Run(async () => { 
                List<CardModel> cards = new();
                CardModelDiscriminator cardDiscriminator = new();
                try 
                {
                    // Connect
                    using (var MtgJsonSqliteConnexion = new SqliteConnection("Data Source=" + App.Config.Path_MtgJsonDownload))
                    {
                        MtgJsonSqliteConnexion.Open();
                        var command = MtgJsonSqliteConnexion.CreateCommand();
                        command.CommandText = SQL_AllCardModels;
                        if (!includeFun) command.CommandText += " AND isFunny=0";
                        // Process
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            string name;
                            string faceName;
                            string type;
                            string text;
                            string keyWords;
                            string power;
                            string toughness;
                            string manacost;
                            float cmc;
                            string colorId;
                            while (await reader.ReadAsync())
                            {
                                name = "";
                                faceName = "";
                                type = "";
                                text = "";
                                keyWords = "";
                                power = "";
                                toughness = "";
                                manacost = "";
                                cmc = 0;
                                colorId = "";
                                // Check
                                name = reader.GetString(0);
                                if (!await reader.IsDBNullAsync(9)) faceName = reader.GetString(9);
                                if (!DiscriminateCardModels(cardDiscriminator,ref name, faceName,reader))
                                {
                                    // Parse
                                    type = reader.GetString(1);
                                    if (!await reader.IsDBNullAsync(2)) text = reader.GetString(2);
                                    if (!await reader.IsDBNullAsync(3)) keyWords = reader.GetString(3);
                                    if (!await reader.IsDBNullAsync(4)) power = reader.GetString(4);
                                    if (!await reader.IsDBNullAsync(5)) toughness = reader.GetString(5);
                                    if (!await reader.IsDBNullAsync(6)) manacost = reader.GetString(6);
                                    cmc = reader.GetFloat(7);
                                    if (!await reader.IsDBNullAsync(8)) colorId = reader.GetString(8);
                                    // Register
                                    cards.Add(new CardModel(name, type, text, keyWords, power, toughness, manacost, cmc, colorId));
                                }
                            }
                        }
                    }
                    // Save
                    using var DB = App.DB.NewContext;
                    {
                        using var transaction = DB.Database.BeginTransaction();
                        await DB.CardModels.AddRangeAsync(cards);
                        await DB.SaveChangesAsync();
                        transaction.Commit();
                    }
                }
                catch (Exception e) { Log.Write(e, "Bulk_CardModels"); }
            });
        }

        private static async Task Bulk_CardVariants(bool isFirstOne, bool includeFun)
        {
            // Retain got cards
            List<User_GotCard> retained = new();
            if (!isFirstOne)
            {
                Log.Write("> RetainGotCards...");
                await RetainGotCards();
            }
            // Delete old data
            using (var DBx = App.DB.NewContext) await DBx.CardVariants.ExecuteDeleteAsync();
            Log.Write("> Importing (Most time heavy operation but last one)...");
            // Insert New Data
            await Task.Run(async () => {
                List<CardVariant> cards = new();
                try
                {
                    // Connect
                    using (var MtgJsonSqliteConnexion = new SqliteConnection("Data Source=" + App.Config.Path_MtgJsonDownload))
                    {
                        MtgJsonSqliteConnexion.Open();
                        var command = MtgJsonSqliteConnexion.CreateCommand();
                        command.CommandText = SQL_AllCardVariants;
                        if (includeFun) command.CommandText += " AND isFunny=0";
                        // Process
                        using (var DBx = App.DB.NewContext) 
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            string id;
                            string rarity;
                            string artist;
                            string lang;
                            string set;
                            string name;
                            string faceName;
                            while (await reader.ReadAsync())
                            {
                                id = "";
                                rarity = "";
                                artist = "";
                                lang = ""; ;
                                set = "";
                                name = "";
                                faceName = "";
                                // Check 
                                id = reader.GetString(0);
                                name = reader.GetString(5);
                                if (!await reader.IsDBNullAsync(6)) faceName = reader.GetString(6);
                                if (!DiscriminateCardVariants(ref name, faceName, reader))
                                {
                                    // Parse
                                    rarity = reader.GetString(1);
                                    if (!await reader.IsDBNullAsync(2)) artist = reader.GetString(2);
                                    lang = reader.GetString(3);
                                    set = reader.GetString(4);
                                    // Register
                                    CardModel CardRef = await DBx.CardModels.Where(x => x.CardId == name).FirstOrDefaultAsync();
                                    cards.Add(new CardVariant(id, rarity, artist, lang, set, CardRef));
                                }
                            }
                        }
                    }
                    // Save
                    using var DB = App.DB.NewContext;
                    {
                        using var transaction = DB.Database.BeginTransaction();
                        await DB.CardVariants.AddRangeAsync(cards);
                        await DB.SaveChangesAsync();
                        transaction.Commit();
                    }
                }
                catch (Exception e) { Log.Write(e); }
            });
            if (!isFirstOne)
            {
                Log.Write("> ReaplyGotCards...");
                await ReaplyGotCards();
            }

        }

        #region Card discrimination

        private static bool DiscriminateCardModels(CardModelDiscriminator cardDiscriminator, ref string name, string faceName, SqliteDataReader reader)
        {
            //double sided

            if (!string.IsNullOrEmpty(faceName) && !name.StartsWith(faceName)) return true;

            // Need rename

            if (name == "Unquenchable Fury")
            {
                string type = reader.GetString(1);
                if (type == "Sorcery") name = "Unquenchable Fury (Battle the Horde)";
                return false;
            }
            if (name == "B.F.M. (Big Furry Monster)")
            {
                string manacost = "";
                if (!reader.IsDBNullAsync(6).Result) manacost = reader.GetString(6);
                name += manacost == "" ? " (Left)" : " (Right)";
                return false;
            }

            // Same Card but different effects(fun)
            if (name == "Scavenger Hunt")
            {
                if (!cardDiscriminator.alreadyAdded_ScavengerHunt) cardDiscriminator.alreadyAdded_ScavengerHunt = true;
                else return true;
            }
            if (name == "The Superlatorium")
            {
                if (!cardDiscriminator.alreadyAdded_TheSuperlatorium) cardDiscriminator.alreadyAdded_TheSuperlatorium = true;
                else return true;
            }
            if (name == "Trivia Contest")
            {
                if (!cardDiscriminator.alreadyAdded_TriviaContest) cardDiscriminator.alreadyAdded_TriviaContest = true;
                else return true;
            }
            if (name == "Ineffable Blessing")
            {
                if (!cardDiscriminator.alreadyAdded_IneffableBlessing) cardDiscriminator.alreadyAdded_IneffableBlessing = true;
                else return true;
            }
            if (name == "Everythingamajig")
            {
                if (!cardDiscriminator.alreadyAdded_Everythingamajig) cardDiscriminator.alreadyAdded_Everythingamajig = true;
                else return true;
            }
            if (name == "Knight of the Kitchen Sink")
            {
                if (!cardDiscriminator.alreadyAdded_KnightoftheKitchenSink) cardDiscriminator.alreadyAdded_KnightoftheKitchenSink = true;
                else return true;
            }
            if (name == "Very Cryptic Command")
            {
                if (!cardDiscriminator.alreadyAdded_VeryCrypticCommand) cardDiscriminator.alreadyAdded_VeryCrypticCommand = true;
                else return true;
            }
            if (name == "Sly Spy")
            {
                if (!cardDiscriminator.alreadyAdded_SlySpy) cardDiscriminator.alreadyAdded_SlySpy = true;
                else return true;
            }
            if (name == "Garbage Elemental")
            {
                if (!cardDiscriminator.alreadyAdded_GarbageElemental) cardDiscriminator.alreadyAdded_GarbageElemental = true;
                else return true;
            }

            return false;
        }

        private static bool DiscriminateCardVariants(ref string name, string faceName, SqliteDataReader reader)
        {
            //double sided
            if (!string.IsNullOrEmpty(faceName) && !name.StartsWith(faceName)) return true;
            // Two different cards with same name
            if (name == "Unquenchable Fury")
            {
                string type = reader.GetString(7);
                if (type == "Sorcery") name = "Unquenchable Fury (Battle the Horde)";
                return false;
            }
            return false;
        }

        private class CardModelDiscriminator
        {
            public bool alreadyAdded_ScavengerHunt = false;
            public bool alreadyAdded_TheSuperlatorium = false;
            public bool alreadyAdded_TriviaContest = false;
            public bool alreadyAdded_IneffableBlessing = false;
            public bool alreadyAdded_Everythingamajig = false;
            public bool alreadyAdded_KnightoftheKitchenSink = false;
            public bool alreadyAdded_VeryCrypticCommand = false;
            public bool alreadyAdded_SlySpy = false;
            public bool alreadyAdded_GarbageElemental = false;
        }

        #endregion

        #region UserData retention

        private async static Task RetainGotCards()
        {
            // Delete old data
            using (var DBx = App.DB.NewContext) await DBx.User_GotCards.ExecuteDeleteAsync();
            // record data
            List<string> errors = new();
            await Task.Run(async () => {
                try
                {
                    List<User_GotCard> listed = new();
                    using var DB = App.DB.NewContext;
                    {
                        List<CardVariant> GotThose = await DB.CardVariants.Where(x => x.Got > 0).Include(x => x.Card).ToListAsync();
                        foreach (var gotThis in GotThose)
                        {
                            if (gotThis.Card == null) errors.Add(("Variant's model not found : "+ gotThis.Id));
                            else
                            {
                                var l = new User_GotCard()
                                {
                                    CardVariantId = gotThis.Id,
                                    CardModelId = gotThis.Card.CardId,
                                    got = gotThis.Got
                                };
                                listed.Add(l);
                            }
                        }
                        using var transaction = DB.Database.BeginTransaction();
                        DB.User_GotCards.AddRange(listed);
                        await DB.SaveChangesAsync();
                        transaction.Commit();
                    }
                }
                catch (Exception e) { Log.Write(e, "RetainGotCards"); }
            });
            //if (errors.Count > 0)
            //{
            //    string errmsg = errors.Count+" errors:";
            //    foreach (var v in errors) errmsg += "- "+ v + "\n";
            //    Log.InformUser("Errors during retain got cards"+ errmsg);
            //}
        }

        private async static Task ReaplyGotCards()
        {
            await Task.Run(async () => {
                try
                {
                    using var DB = App.DB.NewContext;
                    {
                        using var transaction = DB.Database.BeginTransaction();
                        foreach (var gotThis in DB.User_GotCards)
                        {
                            var v = DB.CardVariants.Where(x => x.Id == gotThis.CardVariantId).Include(x => x.Card).FirstOrDefault();
                            if (v != null)
                            {
                                v.Got = gotThis.got;
                                v.Card.Got += gotThis.got;
                                DB.Entry(v).State = EntityState.Modified;
                            }
                        }
                        //DB.User_GotCards.AddRange(listed);
                        await DB.SaveChangesAsync();
                        transaction.Commit();
                    }
                }
                catch (Exception e) { Log.Write(e, "RetainGotCards"); }
            });
        }

        #endregion
        
        #endregion

    }

}
