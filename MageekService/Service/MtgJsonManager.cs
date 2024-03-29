﻿
using MageekCore.Data;
using MageekCore.Data.Collection.Entities;
using MageekCore.Data.Collection;
using MageekCore.Data.Mtg.Entities;
using MageekCore.Data.Mtg;
using Newtonsoft.Json.Linq;
using PlaziatTools;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace MageekCore.Service
{

    public class MtgJsonManager
    {

        const string Url_MtgjsonHash = "https://mtgjson.com/api/v5/AllPrintings.sqlite.sha256";
        const string Url_UpdatePrints = "https://mtgjson.com/api/v5/AllPrintings.sqlite";
        const string Url_UpdatePrices = "https://mtgjson.com/api/v5/AllPrices.json";
        const string Url_UpdatePrecos = "https://mtgjson.com/api/v5/AllDeckFiles.zip";

        private CollectionDbManager collec;
        private MtgDbManager mtg;

        int nbRecPrice = 0;
        int missingPriceEur = 0;
        int missingPriceUsd = 0;
        List<PriceLine> priceList = new();

        public MtgJsonManager(MtgDbManager mtg, CollectionDbManager collec)
        {
            this.collec = collec;
            this.mtg = mtg;
        }

        #region Initialisation

        public async Task<bool> CheckUpdate()
        {
            Logger.Log("Checking...");
            try
            {
                bool? tooOld = FileUtils.IsFileOlder(Folders.File_UpdateOldHash, new TimeSpan(2, 0, 0, 0));
                if (tooOld.HasValue && !tooOld.Value)
                {
                    Logger.Log("Already updated recently");
                    return false;
                }
                bool check = await CheckDbHash();
                Logger.Log(check ? "Update available" : "Already up to date");
                return check;
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return false;
            }
        }

        public async Task<bool> CheckDbHash()
        {
            try
            {
                await HttpUtils.Download(Url_MtgjsonHash, Folders.File_UpdateNewHash);
                bool check = true;
                if (File.Exists(Folders.File_UpdateOldHash))
                {
                    check = FileUtils.ContentDiffers(
                        Folders.File_UpdateNewHash,
                        Folders.File_UpdateOldHash
                    );
                }
                return check;
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return true;
            }
        }

        public async Task DownloadData()
        {
            List<Task> tasks = new()
            {
                HttpUtils.Download(Url_UpdatePrints, Folders.File_UpdatePrints),
                HttpUtils.Download(Url_UpdatePrices, Folders.File_UpdatePrices),
                HttpUtils.Download(Url_UpdatePrecos, Folders.File_UpdatePrecos),
            };
            await Task.WhenAll(tasks);
        }

        public void HashSave()
        {
            try
            {
                File.Copy(Folders.File_UpdateNewHash, Folders.File_UpdateOldHash, true);
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }

        internal async Task FetchData()
        {
            List<Task> tasks = new()
            {
                FetchArchetypes(),
                FetchTranslations(),
                FetchPrecos(),
                FetchPrices(),
            };
        }
        
        #endregion
        
        #region Prints

        private async Task FetchArchetypes()
        {
            Logger.Log("Start...");
            try
            {
                List<ArchetypeCard> archetypes = new();
                Logger.Log("...Parsing...");
                using (MtgDbContext mtgContext = await mtg.GetContext())
                {
                    await Task.Run(() =>
                    {
                        foreach (Cards card in mtgContext.cards)
                        {
                            //if (!(archetypes.Any(x => x.CardUuid == card.Uuid))) //prevent bug in case of duplicated uuid, that already happened
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
                Logger.Log("...Saving...");
                using (CollectionDbContext collecContext = await collec.GetContext())
                {
                    await collecContext.CardArchetypes.ExecuteDeleteAsync();
                    using var transaction = await collecContext.Database.BeginTransactionAsync();
                    await collecContext.CardArchetypes.AddRangeAsync(archetypes);
                    await collecContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                Logger.Log("...Done");
            }
            catch (Exception e)
            {
                Logger.Log("...Error");
                Logger.Log(e);
            }
        }

        private async Task FetchTranslations()
        {
            Logger.Log("Start...");
            try
            {
                List<CardTraduction> traductions = new();
                Logger.Log("...Parsing...");
                using (MtgDbContext mtgContext = await mtg.GetContext())
                {
                    foreach (CardForeignData traduction in mtgContext.cardForeignData)
                    {
                        await Task.Run(() =>
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
                                            ? traduction.FaceName.RemoveDiacritics().Replace('-', ' ').ToLower()
                                            : traduction.FaceName
                                    }
                                );
                            }
                        });
                    }
                }
                Logger.Log("...Saving...");
                using (CollectionDbContext collecContext = await collec.GetContext())
                {
                    await collecContext.CardTraductions.ExecuteDeleteAsync();
                    using var transaction = await collecContext.Database.BeginTransactionAsync();
                    await collecContext.CardTraductions.AddRangeAsync(traductions);
                    await collecContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                Logger.Log("...Done");
            }
            catch (Exception e)
            {
                Logger.Log("...Error");
                Logger.Log(e);
            }
        }

        #endregion

        #region Precos

        private async Task FetchPrecos()
        {
            Logger.Log("Start...");
            Logger.Log("...Uncompressing...");
            System.IO.Compression.ZipFile.ExtractToDirectory(Folders.File_UpdatePrecos, Folders.TempPrecoFolder, overwriteFiles: true);
            Logger.Log("...Parsing...");
            await ParsePrecos(Folders.TempPrecoFolder, Folders.File_Precos);
            Logger.Log("...Cleaning");
            Directory.Delete(Folders.TempPrecoFolder, true);
            Logger.Log("...Done");
        }

        private async Task ParsePrecos(string tmpPath, string finalPath)
        {
            // READ
            List<Preco> list = new List<Preco>();
            foreach (string precoPath in Directory.GetFiles(tmpPath))
            {
                try
                {
                    list.Add(await ParsePreco(precoPath));
                }
                catch (Exception e) { Logger.Log(e); }
            }
            // WRITE
            Console.WriteLine(DateTime.Now);
            var options = new JsonSerializerOptions { IncludeFields = true };
            string jsonString = JsonSerializer.Serialize(list, options);
            File.WriteAllText(finalPath, jsonString);
        }

        private async Task<Preco> ParsePreco(string precoPath)
        {
            using StreamReader reader = new(precoPath);
            string jsonString = await reader.ReadToEndAsync();
            dynamic dynData = JObject.Parse(jsonString);

            string code = dynData.data.code;
            string name = dynData.data.name;
            string releaseDate = dynData.data.releaseDate;
            string type = dynData.data.type;

            List<Tuple<string, int>> CommanderCardUuids = new List<Tuple<string, int>>();
            foreach (dynamic card in dynData.data.commander)
            {
                string uuid = card.uuid;
                int quantity = card.count;
                CommanderCardUuids.Add(new Tuple<string, int>(uuid, quantity));
            }

            List<Tuple<string, int>> mainCardUuids = new List<Tuple<string, int>>();
            foreach (dynamic card in dynData.data.mainBoard)
            {
                string uuid = card.uuid;
                int quantity = card.count;
                mainCardUuids.Add(new Tuple<string, int>(uuid, quantity));
            }

            List<Tuple<string, int>> sideCardUuids = new List<Tuple<string, int>>();
            foreach (dynamic card in dynData.data.sideBoard)
            {
                string uuid = card.uuid;
                int quantity = card.count;
                sideCardUuids.Add(new Tuple<string, int>(uuid, quantity));
            }

            return new Preco()
            {
                Title = name,
                Code = code,
                ReleaseDate = releaseDate,
                Kind = type,
                CommanderCardUuids = CommanderCardUuids,
                MainCardUuids = mainCardUuids,
                SideCardUuids = sideCardUuids
            };

        }

        #endregion

        #region Prices

        // Pretty clunky, probably could be enhanced

        public async Task FetchPrices()
        {
            Logger.Log("Start...");
            try
            {
                using (CollectionDbContext collecContext = await collec.GetContext())
                {
                    await collecContext.PriceLine.ExecuteDeleteAsync();
                    await collecContext.SaveChangesAsync();
                }
                using (FileStream s = File.Open(Folders.File_UpdatePrices, FileMode.Open))
                using (StreamReader sr = new StreamReader(s))
                using (Newtonsoft.Json.JsonReader reader = new Newtonsoft.Json.JsonTextReader(sr))
                {
                    while (reader.Read())
                    {
                        if (reader.TokenType == Newtonsoft.Json.JsonToken.StartObject)
                        {
                            bool metaSkip = false;
                            while (reader.Read())
                            {
                                if (reader.TokenType == Newtonsoft.Json.JsonToken.StartObject)
                                {
                                    if (!metaSkip) metaSkip = true;
                                    else await ProcessPriceData(reader);
                                }
                            }
                        }
                    }
                }

                if (priceList.Count > 0)
                {
                    nbRecPrice += priceList.Count;
                    using (CollectionDbContext collecContext = await collec.GetContext())
                    {
                        await collecContext.PriceLine.AddRangeAsync(priceList);
                        await collecContext.SaveChangesAsync();
                    }
                    priceList.Clear();
                }
                Logger.Log("Results - " + nbRecPrice + " records processed, missing eur : " + missingPriceEur+ ", missing usd : "+missingPriceUsd+".");
                Logger.Log("...Done");
            }
            catch (Exception e)
            {
                Logger.Log("...Error");
                Logger.Log(e);

            }
        }

        private async Task ProcessPriceData(Newtonsoft.Json.JsonReader reader)
        {
            while (await reader.ReadAsync())
            {
                if (reader.TokenType == Newtonsoft.Json.JsonToken.PropertyName)
                {
                    string UUID = reader.Value.ToString();
                    await RecordPrice(GetPriceLine(UUID, reader));
                }
            }
        }

        private async Task RecordPrice(PriceLine priceLine)
        {
            if (priceLine == null) return;
            priceList.Add(priceLine);
            if(priceList.Count >= 1000)
            {
                nbRecPrice+=1000;
                using (CollectionDbContext collecContext = await collec.GetContext())
                {
                    await collecContext.PriceLine.AddRangeAsync(priceList);
                    await collecContext.SaveChangesAsync();
                }
                priceList.Clear();
            }
        }

        private PriceLine? GetPriceLine(string UUID, Newtonsoft.Json.JsonReader reader)
        {
            //Console.WriteLine(">>> " + UUID);
            string CardBlock = RegisterBlock(reader);
            return ParsePriceBlock(UUID, CardBlock);
        }

        private string RegisterBlock(Newtonsoft.Json.JsonReader reader)
        {
            string data = string.Empty;
            int deepness = 0;
            int lastdeepness = -1;
            bool starting = true;
            data += "{";
            while ((deepness >= 1 || starting == true) && reader.Read())
            {
                if (reader.TokenType == Newtonsoft.Json.JsonToken.PropertyName)
                {
                    if (lastdeepness > deepness) data += ",";
                    data += "\"" + reader.Value.ToString() + "\":";
                }
                else if (reader.TokenType == Newtonsoft.Json.JsonToken.StartObject)
                {
                    if (deepness > 0) data += "{";
                    deepness++;
                    if (deepness > 1) starting = false;
                }
                else if (reader.TokenType == Newtonsoft.Json.JsonToken.EndObject)
                {
                    lastdeepness = deepness;
                    if (deepness > 1)
                    {
                        if (data.EndsWith(",")) data = data.Remove(data.Length - 1);
                        data += "}";
                    }
                    deepness--;
                }
                else
                {
                    string value = reader.Value.ToString();
                    if (value == "USD") data += "\"USD\"";
                    else if (value == "EUR") data += "\"EUR\"";
                    else data += value.Replace(",", ".") + ",";
                }
            }
            data += "}";
            data = data.Replace("\"\"", "\",\"");
            return data;
        }

        private PriceLine ParsePriceBlock(string UUID, string cardBlock)
        {
            PriceLine line = new PriceLine() { CardUuid = UUID };
            dynamic dynData = JObject.Parse(cardBlock);
            try
            {
                if (dynData.paper != null)
                {
                    if (dynData.paper.cardmarket != null)
                    {
                        dynamic retail = dynData.paper.cardmarket.retail;
                        if (retail != null)
                        {
                            string s = retail.ToString().Replace("}", "");
                            line.PriceEurAccrossTime = s;
                            string[] splitted = s.Split(",");
                            string last = splitted[splitted.Length - 1].Split(":").Last();
                            line.LastPriceEur = float.Parse(last.Trim().Replace(".", ","));
                        }
                    }
                    else missingPriceEur++;
                    if (dynData.paper.cardsphere != null)
                    {
                        dynamic retail = dynData.paper.cardsphere.retail;
                        if (retail != null)
                        {
                            string s = retail.ToString().Replace("}", "");
                            line.PriceUsdAccrossTime = s;
                            string[] splitted = s.Split(",");
                            string last = splitted[splitted.Length - 1].Split(":").Last();
                            line.LastPriceUsd = float.Parse(last.Trim().Replace(".", ","));
                        }
                    }
                    else missingPriceUsd++;
                    return line;
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return null;
        }

        #endregion

    }

}
