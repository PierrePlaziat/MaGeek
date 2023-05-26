using ScryfallApi.Client.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Net.Http;
using System.Windows.Media.Imaging;
using MaGeek.Entities;
using MaGeek.Framework.Data;
using MaGeek.Framework;

namespace MaGeek.AppBusiness
{

    /// <summary>
    /// ScryfallApi data to local DB
    /// TODO : maybe move unchanging data to bulk importer completely
    /// </summary>
    public static class MageekApi
    {

        const int DelayApi = 150;
        static readonly Random rnd = new();

        public static async Task<List<CardLegality>> GetLegalities(CardModel card)
        {
            List<CardLegality> legalities = new();
            if (card == null) return legalities;
            try
            {
                await RetrieveLegalities(card);
                using var DB = App.DB.GetNewContext();
                legalities = await DB.CardLegalities.Where(x => x.CardId == card.CardId).ToListAsync();
            }
            catch (Exception e) { Log.Write(e); }
            return legalities;
        }
        public static async Task<List<CardRule>> GetRules(CardModel card)
        {
            List<CardRule> rules = new();
            if (card == null) return rules;
            try
            {
                await RetrieveLegalities(card);
                using var DB = App.DB.GetNewContext();
                rules = await DB.CardRules.Where(x => x.CardId == card.CardId).ToListAsync();
            }
            catch (Exception e) { Log.Write(e); }
            return rules;
        }
        public static async Task<List<CardRelation>> GetRelatedCards(CardModel card)
        {
            List<CardRelation> relatedCards = new();
            if (card == null) return relatedCards;
            try
            {
                await RetrieveRelatedCards(card);
                using var DB = App.DB.GetNewContext();
                relatedCards = await DB.CardRelations.Where(x => x.Card1Id == card.CardId)
                    .ToListAsync();
            }
            catch (Exception e) { Log.Write(e); }
            return relatedCards;
        }

        private static async Task RetrieveLegalities(CardModel card)
        {
            if (card == null) return;
            if (!IsLegalitiesOutdated(card)) return;
            try
            {
                await DestroyLegalitiesRecords(card);
                Thread.Sleep(DelayApi);
                string json_data = await HttpUtils.Get("https://api.scryfall.com/cards/" + card.Variants[0].Id);
                Card scryfallCard = JsonSerializer.Deserialize<Card>(json_data);
                await SaveLegality(card, scryfallCard.Legalities);
                await SaveRulings(card, await RetrieveRulings(card,scryfallCard.RulingsUri));
            }
            catch (Exception e) { Log.Write(e); }
        }
        private static async Task<dynamic> RetrieveRulings(CardModel card,Uri rulingsUri)
        {
            if (rulingsUri==null) return null;
            try
            {
                await DestroyRulings(card);
                Thread.Sleep(DelayApi);
                string json_data = await HttpUtils.Get(rulingsUri.ToString());
                ResultList<CardRule> rules = JsonSerializer.Deserialize<ResultList<CardRule>>(json_data);
                return rules;
            }
            catch (Exception e) { Log.Write(e); }
            return null;
        }
        private static async Task RetrieveRelatedCards(CardModel card)
        {
            if (card == null) return;
            if (!IsRelatedCardsOutdated(card)) return;
            try
            {
                await DestroyRelatedsRecords(card);
                Thread.Sleep(DelayApi);
                string json_data = await HttpUtils.Get("https://api.scryfall.com/cards/" + card.Variants[0].Id);
                Card scryfallCard = JsonSerializer.Deserialize<Card>(json_data);
                List<CardRelation> rels = new();
                if (scryfallCard.AllParts != null)
                {
                    foreach (var part in scryfallCard.AllParts)
                    {
                        part.TryGetValue("name", out string cardName);

                        if (!cardName.StartsWith("A-"))
                        {
                            part.TryGetValue("id", out string cardId);
                            part.TryGetValue("component", out string relationType);

                            if (cardName != card.CardId)
                            {
                                CardModel relatedCard;
                                using (var DB = App.DB.GetNewContext())
                                {
                                    relatedCard = await DB.CardModels.Where(x => x.CardId == cardName)
                                            .FirstOrDefaultAsync();
                                }
                                if (relatedCard == null)
                                {
                                    // TODO tokens gestion
                                    //var retrieved = await HttpUtils.Get("https://api.scryfall.com/cards/" + cardId);
                                    //Card result = JsonSerializer.Deserialize<Card>(retrieved);
                                    //await RecordCard(result, false);
                                    //using (var DB = App.DB.GetNewContext())
                                    //{
                                    //    relatedCard = await DB.CardModels.Where(x => x.CardId == cardName)
                                    //            .FirstOrDefaultAsync();
                                    //}
                                }
                                if (relatedCard != null)
                                {
                                    rels.Add(
                                        new CardRelation()
                                        {
                                            Card1Id = card.CardId,
                                            Card2Id = relatedCard.CardId,
                                            RelationType = relationType,
                                            LastUpdate = DateTime.Now.ToShortDateString()
                                        }
                                    );
                                }
                                else
                                {
                                    Log.Write("Couldnt retrieve related card");
                                }
                            }
                        }
                    }
                    await SaveRelatedCards(card, rels);
                    App.Events.RaiseUpdateCardCollec();
                }
            }
            catch (Exception e) { Log.Write(e); }
        }
        public static async Task RetrieveCardValues(CardVariant card)
        {
            if (card == null) return;
            if (!IsCardValuesOutdated(card)) return;
            try
            {
                Thread.Sleep(DelayApi);
                string json_data = await HttpUtils.Get("https://api.scryfall.com/cards/" + card.Id);
                Card scryfallCard = JsonSerializer.Deserialize<Card>(json_data);
                await SavePrice(card, scryfallCard.Prices, scryfallCard.EdhrecRank);
            }
            catch (Exception e) { Log.Write(e); }
        }

        private static bool IsLegalitiesOutdated(CardModel card)
        {
            DateTime lastUpdate;
            using (var DB = App.DB.GetNewContext())
            {
                var legality = DB.CardLegalities.Where(x => x.CardId == card.CardId).FirstOrDefault();
                if (legality == null) return true;
                if (string.IsNullOrEmpty(legality.LastUpdate)) return true;
                lastUpdate = DateTime.Parse(legality.LastUpdate);
            }
            if (lastUpdate < DateTime.Now.AddDays(-7)) return true;
            else return false;
        }
        private static bool IsRelatedCardsOutdated(CardModel card)
        {
            DateTime lastUpdate;
            using (var DB = App.DB.GetNewContext())
            {
                var relation = DB.CardRelations.Where(x => x.Card1Id == card.CardId).FirstOrDefault();
                if (relation == null) return true;
                if (string.IsNullOrEmpty(relation.LastUpdate)) return true;
                lastUpdate = DateTime.Parse(relation.LastUpdate);
            }
            if (lastUpdate < DateTime.Now.AddMonths(-1)) return true;
            else return false;
        }
        private static bool IsCardValuesOutdated(CardVariant card)
        {
            if (string.IsNullOrEmpty(card.LastUpdate)) return true;
            DateTime lastUpdate = DateTime.Parse(card.LastUpdate);
            if (lastUpdate < DateTime.Now.AddDays(-1)) return true;
            else return false;
        }

        private static async Task SavePrice(CardVariant cardVariant, Price price, int edhRank)
        {
            try
            {
                using var DB = App.DB.GetNewContext();
                if (price != null)
                {
                    if (price.Eur != null) cardVariant.ValueEur = price.Eur.ToString();
                    if (price.Usd != null) cardVariant.ValueUsd = price.Usd.ToString();
                    cardVariant.EdhRecRank = edhRank;
                    cardVariant.LastUpdate = DateTime.Now.ToShortDateString();
                }
                DB.Entry(cardVariant).State = EntityState.Modified;
                await DB.SaveChangesAsync();
            }
            catch (Exception e) { Log.Write(e); }
        }
        private static async Task SaveLegality(CardModel card, Dictionary<string, string> legalityDico)
        {
            if (legalityDico == null) return;
            try
            {
                List<CardLegality> legal = new();
                foreach (var l in legalityDico)
                {
                    legal.Add(new CardLegality()
                    {
                        Format = l.Key,
                        IsLegal = l.Value,
                        CardId = card.CardId,
                        LastUpdate = DateTime.Now.ToShortDateString(),
                    });
                }
                using var DB = App.DB.GetNewContext();
                {
                    DB.CardLegalities.AddRange(legal);
                    await DB.SaveChangesAsync();
                }
            }
            catch (Exception e) { Log.Write(e); }
        }
        private static async Task SaveRulings(CardModel card, ResultList<CardRule> rulings)
        {
            try
            {
                List<CardRule> rule = new();
                foreach (var l in rulings.Data)
                {
                    l.CardId = card.CardId;
                    rule.Add(l);
                }
                using var DB = App.DB.GetNewContext();
                {
                    DB.CardRules.AddRange(rule);
                    await DB.SaveChangesAsync();
                }
            }
            catch (Exception e) { Log.Write(e); }
        }
        private static async Task SaveRelatedCards(CardModel card, List<CardRelation> rels)
        {
            try
            {
                using var DB = App.DB.GetNewContext();
                await DB.CardRelations.AddRangeAsync(rels);
                DB.Entry(card).State = EntityState.Unchanged;
                await DB.SaveChangesAsync();
            }
            catch (Exception e) { Log.Write(e); }
        }

        private static async Task DestroyLegalitiesRecords(CardModel localCard)
        {
            try
            {
                using var DB = App.DB.GetNewContext();
                IEnumerable<CardLegality> existingValues = DB.CardLegalities.Where(x => x.CardId == localCard.CardId);
                if (existingValues.Any())
                {
                    DB.CardLegalities.RemoveRange(existingValues);
                    await DB.SaveChangesAsync();
                }
            }
            catch (Exception e) { Log.Write(e); }
        }
        private static async Task DestroyRelatedsRecords(CardModel localCard)
        {
            try
            {
                using var DB = App.DB.GetNewContext();
                List<CardRelation> existingValues = await DB.CardRelations.Where(x => x.Card1Id == localCard.CardId).ToListAsync();
                if (existingValues == null || existingValues.Count == 0) return;
                DB.CardRelations.RemoveRange(existingValues);
                await DB.SaveChangesAsync();
            }
            catch (Exception e) { Log.Write(        e); }
        }
        private static async Task DestroyRulings(CardModel localCard)
        {
            try
            {
                using var DB = App.DB.GetNewContext();
                List<CardRule> existingValues = await DB.CardRules.Where(x => x.CardId == localCard.CardId).ToListAsync();
                if (existingValues == null || existingValues.Count == 0) return;
                DB.CardRules.RemoveRange(existingValues);
                await DB.SaveChangesAsync();
            }
            catch (Exception e) { Log.Write(        e); }
        }

        public static async Task<BitmapImage> RetrieveImage(CardVariant magicCardVariant, bool back = false, int nbTry = 0)
        {
            if (string.IsNullOrEmpty(magicCardVariant.ImageUrl_Front))
            {
                await GetImgUrls(magicCardVariant);
            }
            BitmapImage img = null;
            try
            {
                var taskCompletion = new TaskCompletionSource<BitmapImage>();
                string localFileName = "";
                //if (magicCardVariant.IsCustom == 0)
                {
                    if (back)
                    {
                        localFileName = Path.Combine(App.Config.Path_ImageFolder, magicCardVariant.Id + "_back.png");
                        if (!File.Exists(localFileName))
                        {
                            var httpClient = new HttpClient();
                            using var stream = await httpClient.GetStreamAsync(magicCardVariant.ImageUrl_Back);
                            using var fileStream = new FileStream(localFileName, FileMode.Create);
                            await stream.CopyToAsync(fileStream);
                        }
                    }
                    else
                    {
                        localFileName = Path.Combine(App.Config.Path_ImageFolder, magicCardVariant.Id + ".png");
                        if (!File.Exists(localFileName))
                        {
                            var httpClient = new HttpClient();
                            using var stream = await httpClient.GetStreamAsync(magicCardVariant.ImageUrl_Front);
                            using var fileStream = new FileStream(localFileName, FileMode.Create);
                            await stream.CopyToAsync(fileStream);
                        }
                    }
                }
                var path = Path.GetFullPath(localFileName);
                Uri imgUri = new("file://" + path, UriKind.Absolute);
                img = new BitmapImage(imgUri);
                taskCompletion.SetResult(img);
            }

            catch (Exception e)
            {
                Log.Write(e);
                await Task.Run(() => {
                    Thread.Sleep(rnd.Next(10) * 50);
                });
                if (nbTry < 3) return await RetrieveImage(magicCardVariant, back, nbTry++);
            }
            return img;
        }
        private static async Task GetImgUrls(CardVariant card)
        {
            if (card == null) return;
            try
            {
                Card scryCard = null;
                Thread.Sleep(DelayApi);
                string json_data = await HttpUtils.Get("https://api.scryfall.com/cards/" + card.Id);
                scryCard = JsonSerializer.Deserialize<Card>(json_data);

                using (var DB = App.DB.GetNewContext())
                {
                    if (scryCard.ImageUris != null)
                    {
                        card.ImageUrl_Front = scryCard.ImageUris.Values.LastOrDefault().ToString();
                        card.ImageUrl_Back = "";
                        DB.Entry(card).State = EntityState.Modified;
                        await DB.SaveChangesAsync();
                    }
                    else if (scryCard.CardFaces != null)
                    {
                        card.ImageUrl_Front = scryCard.CardFaces[0].ImageUris.Values.LastOrDefault().ToString();
                        card.ImageUrl_Back = scryCard.CardFaces[1].ImageUris.Values.LastOrDefault().ToString();
                        DB.Entry(card).State = EntityState.Modified;
                        await DB.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e) { Log.Write(e); }


        }

        // TODO : use MtgJson for sets and tokens
        #region Old

        //public static async Task<List<Set>> RetrieveSets()
        //{
        //    List<Set> sets = new();
        //    try
        //    {
        //        Thread.Sleep(DelayApi);
        //        string data = await HttpUtils.Get("https://api.scryfall.com/sets/");
        //        var result = JsonSerializer.Deserialize<ResultList<Set>>(data);
        //        sets.AddRange(result.Data);
        //    }
        //    catch (Exception e) { AppLogger.ShowError(MethodBase.GetCurrentMethod().Name, e); }
        //    return sets;
        //}
        //public static async Task<List<Card>> RetrieveSetCards(string setCode)
        //{
        //    List<Card> cards = new();
        //    try
        //    {
        //        ResultList<Card> result = null;
        //        do
        //        {
        //            Thread.Sleep(DelayApi);
        //            string data = "";
        //            if (result == null) data = await HttpUtils.Get("https://api.scryfall.com/cards/search?order=cmc&q=e:" + setCode);
        //            else data = await HttpUtils.Get(result.NextPage.ToString());
        //            result = JsonSerializer.Deserialize<ResultList<Card>>(data);
        //            cards.AddRange(result.Data);
        //        }
        //        while (result.HasMore);
        //    }
        //    catch (Exception e) { AppLogger.ShowError(MethodBase.GetCurrentMethod().Name, e); }
        //    return cards;
        //}

        //public static async Task<List<Card>> RetrieveCard(string cardName, bool exactName, bool skipIfExists, bool includeForeign)
        //{
        //    List<Card> cards = new();
        //    try
        //    {
        //        if (skipIfExists)
        //        {
        //            using (var DB = App.DB.GetNewContext())
        //            {
        //                if (DB.CardModels.Where(x => x.CardId == cardName).Any()) return new List<Card>();
        //            }
        //        }
        //        cards = await RetrieveCard(cardName);
        //        if (!cards.Any())
        //        {
        //            if (includeForeign)
        //            {
        //                string newname = await MageekCollection.GetEnglishNameFromForeignName(cardName, "french");
        //                if (newname != null) cardName = newname;
        //                if (newname != "") cards = await RetrieveCard(cardName);
        //            }
        //        }
        //        if (exactName)
        //        {
        //            cards = await FilterExactName(cards, cardName);
        //        }
        //    }
        //    catch (Exception e) { AppLogger.ShowError(MethodBase.GetCurrentMethod().Name, e); }
        //    return cards;
        //}
        //private static async Task<List<Card>> RetrieveCard(string cardName)
        //{
        //    List<Card> cards = new();
        //    try
        //    {
        //        ResultList<Card> result = null;
        //        do
        //        {
        //            Thread.Sleep(DelayApi);
        //            string data = "";
        //            if (result == null) data = await HttpUtils.Get("https://api.scryfall.com/cards/search?order=cmc&q=" + cardName + "+unique:prints");
        //            else data = await HttpUtils.Get(result.NextPage.ToString());
        //            result = JsonSerializer.Deserialize<ResultList<Card>>(data);
        //            if (result.Data != null) cards.AddRange(result.Data);
        //        }
        //        while (result.HasMore);
        //    }
        //    catch (Exception e) { AppLogger.ShowError(MethodBase.GetCurrentMethod().Name, e); }
        //    return cards;
        //}
        //private static async Task<List<Card>> FilterExactName(List<Card> cards, string cardName)
        //{
        //    List<Card> filteredCards = new();
        //    await Task.Run(() =>
        //    {
        //        foreach (var card in cards)
        //        {
        //            if (IsExactCardName(cardName.ToLower(), card.Name.ToLower()))
        //                filteredCards.Add(card);
        //        }
        //    });
        //    return filteredCards;
        //}
        //private static bool IsExactCardName(string askedCardName, string retrievedCardName)
        //{
        //    string[] cardNames = askedCardName.Split(" // "); // doubled sided card names
        //    foreach (string name in cardNames)
        //    {
        //        if (name == retrievedCardName) return true;
        //    }
        //    return false;
        //}

        //public static async Task RecordCard(Card scryCard, bool Owned)
        //{
        //    try
        //    {
        //        if (scryCard.Name.StartsWith("A-")) return;
        //        CardModel localCard;
        //        CardVariant localVariant;
        //        using (var DB = App.DB.GetNewContext())
        //        {
        //            // Card
        //            localCard = DB.CardModels.Where(x => x.CardId == scryCard.Name)
        //                                .Include(x => x.Variants)
        //                                .FirstOrDefault();
        //            if (localCard == null)
        //            {
        //                localCard = new CardModel(scryCard);
        //                DB.CardModels.Add(localCard);
        //                await DB.SaveChangesAsync();
        //            }
        //            // Variant
        //            localVariant = localCard.Variants.Where(x => x.Id == scryCard.Id.ToString()).FirstOrDefault();
        //            if (localVariant == null)
        //            {
        //                localVariant = new CardVariant(scryCard);
        //                if (Owned) localVariant.Got++;
        //                localVariant.Card = localCard;
        //                localCard.Variants.Add(localVariant);
        //                DB.CardVariants.Add(localVariant);
        //                DB.Entry(localVariant).State = EntityState.Added;
        //                DB.Entry(localCard).State = EntityState.Modified;
        //                await DB.SaveChangesAsync();
        //            }
        //        }
        //    }
        //    catch (Exception e) { AppLogger.ShowError(MethodBase.GetCurrentMethod().Name, e); }
        //}

        #endregion

    }

}