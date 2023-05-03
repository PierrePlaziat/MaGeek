﻿using MaGeek.AppData.Entities;
using MaGeek.AppData;
using Plaziat.CommonWpf;
using ScryfallApi.Client.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Net.Http;
using System.Windows.Media.Imaging;
using ScryfallApi.Client.Apis;

namespace MaGeek.AppBusiness
{

    public static class MageekApi
    {

        const int DelayApi = 150;
        static Random rnd = new Random();


        #region Static data

        public static async Task<List<Set>> RetrieveSets()
        {
            List<Set> sets = new();
            try
            {
                Thread.Sleep(DelayApi);
                string data = await HttpUtils.Get("https://api.scryfall.com/sets/");
                var result = JsonSerializer.Deserialize<ResultList<Set>>(data);
                sets.AddRange(result.Data);
            }
            catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }
            return sets;
        }
        public static async Task<List<Card>> RetrieveSetCards(string setCode)
        {
            List<Card> cards = new();
            try
            {
                ResultList<Card> result = null;
                do
                {
                    Thread.Sleep(DelayApi);
                    string data = "";
                    if (result == null) data = await HttpUtils.Get("https://api.scryfall.com/cards/search?order=cmc&q=e:" + setCode);
                    else data = await HttpUtils.Get(result.NextPage.ToString());
                    result = JsonSerializer.Deserialize<ResultList<Card>>(data);
                    cards.AddRange(result.Data);
                }
                while (result.HasMore);
            }
            catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }
            return cards;
        }

        public static async Task<List<Card>> RetrieveCard(string cardName, bool exactName, bool skipIfExists, bool includeForeign)
        {
            List<Card> cards = new();
            try
            {
                if (skipIfExists)
                {
                    using (var DB = App.DB.GetNewContext())
                    {
                        if (DB.Cards.Where(x => x.CardId == cardName).Any()) return new List<Card>();
                    }
                }
                cards = await RetrieveCard(cardName);
                if (!cards.Any())
                {
                    if (includeForeign)
                    {
                        string newname = await MageekTranslator.GetEnglishNameFromForeignName(cardName, "french");
                        if (newname != null) cardName = newname;
                        if (newname != "") cards = await RetrieveCard(cardName);
                    }
                }
                if (exactName)
                {
                    cards = await FilterExactName(cards, cardName);
                }
            }
            catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }
            return cards;
        }
        private static async Task<List<Card>> RetrieveCard(string cardName)
        {
            List<Card> cards = new();
            try
            {
                ResultList<Card> result = null;
                do
                {
                    Thread.Sleep(DelayApi);
                    string data = "";
                    if (result == null) data = await HttpUtils.Get("https://api.scryfall.com/cards/search?order=cmc&q=" + cardName + "+unique:prints");
                    else data = await HttpUtils.Get(result.NextPage.ToString());
                    result = JsonSerializer.Deserialize<ResultList<Card>>(data);
                    if (result.Data != null) cards.AddRange(result.Data);
                }
                while (result.HasMore);
            }
            catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }
            return cards;
        }
        private static async Task<List<Card>> FilterExactName(List<Card> cards, string cardName)
        {
            List<Card> filteredCards = new();
            await Task.Run(() =>
            {
                foreach (var card in cards)
                {
                    if (IsExactCardName(cardName.ToLower(), card.Name.ToLower()))
                        filteredCards.Add(card);
                }
            });
            return filteredCards;
        }
        private static bool IsExactCardName(string askedCardName, string retrievedCardName)
        {
            string[] cardNames = askedCardName.Split(" // "); // doubled sided card names
            foreach (string name in cardNames)
            {
                if (name == retrievedCardName) return true;
            }
            return false;
        }

        public static async Task RecordCards(List<Card> cardlist, bool owned = false)
        {
            foreach (var card in cardlist) await RecordCard(card, owned);
        }

        public static async Task RecordCard(Card scryCard, bool Owned)
        {
            try
            {
                if (scryCard.Name.StartsWith("A-")) return;
                MagicCard localCard;
                MagicCardVariant localVariant;
                using (var DB = App.DB.GetNewContext())
                {
                    // Card
                    localCard = DB.Cards.Where(x => x.CardId == scryCard.Name)
                                        .Include(x => x.Variants)
                                        .FirstOrDefault();
                    if (localCard == null)
                    {
                        localCard = new MagicCard(scryCard);
                        DB.Cards.Add(localCard);
                        await DB.SaveChangesAsync();
                    }
                    // Variant
                    localVariant = localCard.Variants.Where(x => x.Id == scryCard.Id.ToString()).FirstOrDefault();
                    if (localVariant == null)
                    {
                        localVariant = new MagicCardVariant(scryCard);
                        if (Owned) localVariant.Got++;
                        localVariant.Card = localCard;
                        localCard.Variants.Add(localVariant);
                        DB.CardVariants.Add(localVariant);
                        DB.Entry(localVariant).State = EntityState.Added;
                        DB.Entry(localCard).State = EntityState.Modified;
                        await DB.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }
        }

        public static async Task<BitmapImage> RetrieveImage(MagicCardVariant magicCardVariant, bool back = false, int nbTry = 0)
        {
            if(string.IsNullOrEmpty(magicCardVariant.ImageUrl_Front))
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
                await Task.Run(() => {
                    Thread.Sleep(rnd.Next(10) * 50);
                });
                if (nbTry < 3) return await RetrieveImage(magicCardVariant, back, nbTry++);
            }
            return img;
        }

        private static async Task GetImgUrls(MagicCardVariant card)
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
                        DB.Entry(card).State= EntityState.Modified;
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
            catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }


        }

        #endregion

        #region Dynamic data

        public static async Task<List<Legality>> GetLegalities(MagicCard card)
        {
            List<Legality> legalities = new();
            if (card == null) return legalities;
            try
            {
                await RetrieveLegalities(card);
                using var DB = App.DB.GetNewContext();
                legalities = await DB.Legalities.Where(x => x.CardId == card.CardId).ToListAsync();
            }
            catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }
            return legalities;
        }
        public static async Task<List<Rule>> GetRules(MagicCard card)
        {
            List<Rule> rules = new();
            if (card == null) return rules;
            try
            {
                await RetrieveLegalities(card);
                using var DB = App.DB.GetNewContext();
                rules = await DB.CardRules.Where(x => x.CardId == card.CardId).ToListAsync();
            }
            catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }
            return rules;
        }
        public static async Task<List<CardCardRelation>> GetRelatedCards(MagicCard card)
        {
            List<CardCardRelation> relatedCards = new();
            if (card == null) return relatedCards;
            try
            {
                await RetrieveRelatedCards(card);
                using var DB = App.DB.GetNewContext();
                relatedCards = await DB.CardRelations.Where(x => x.Card1Id == card.CardId)
                    .ToListAsync();
            }
            catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }
            return relatedCards;
        }

        private static async Task RetrieveLegalities(MagicCard card)
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
            catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }
        }
        
        private static async Task<dynamic> RetrieveRulings(MagicCard card,Uri rulingsUri)
        {
            if (rulingsUri==null) return null;
            try
            {
                await DestroyRulings(card);
                Thread.Sleep(DelayApi);
                string json_data = await HttpUtils.Get(rulingsUri.ToString());
                ResultList<Rule> rules = JsonSerializer.Deserialize<ResultList<Rule>>(json_data);
                return rules;
            }
            catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }
            return null;
        }

        private static async Task RetrieveRelatedCards(MagicCard card)
        {
            if (card == null) return;
            if (!IsRelatedCardsOutdated(card)) return;
            try
            {
                await DestroyRelatedsRecords(card);
                Thread.Sleep(DelayApi);
                string json_data = await HttpUtils.Get("https://api.scryfall.com/cards/" + card.Variants[0].Id);
                Card scryfallCard = JsonSerializer.Deserialize<Card>(json_data);
                List<CardCardRelation> rels = new();
                if (scryfallCard.AllParts != null)
                {
                    foreach (var part in scryfallCard.AllParts)
                    {
                        string cardName;
                        part.TryGetValue("name", out cardName);

                        if (cardName.StartsWith("A-"))
                        {
                            string cardId;
                            string relationType;
                            part.TryGetValue("id", out cardId);
                            part.TryGetValue("component", out relationType);

                            if (cardName != card.CardId)
                            {
                                MagicCard relatedCard;
                                using (var DB = App.DB.GetNewContext())
                                {
                                    relatedCard = await DB.Cards.Where(x => x.CardId == cardName)
                                            .FirstOrDefaultAsync();
                                }
                                if (relatedCard == null)
                                {
                                    var retrieved = await HttpUtils.Get("https://api.scryfall.com/cards/" + cardId);
                                    Card result = JsonSerializer.Deserialize<Card>(retrieved);
                                    await RecordCard(result, false);
                                    using (var DB = App.DB.GetNewContext())
                                    {
                                        relatedCard = await DB.Cards.Where(x => x.CardId == cardName)
                                                .FirstOrDefaultAsync();
                                    }
                                }
                                if (relatedCard != null)
                                {
                                    rels.Add(
                                        new CardCardRelation()
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
                                    MessageBoxHelper.ShowMsg("couldnt retrieve related card");
                                }
                            }
                        }
                    }
                    await SaveRelatedCards(card, rels);
                    App.Events.RaiseUpdateCardCollec();
                }
            }
            catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }
        }
        public static async Task RetrieveCardValues(MagicCardVariant card)
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
            catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }
        }

        private static bool IsLegalitiesOutdated(MagicCard card)
        {
            DateTime lastUpdate;
            using (var DB = App.DB.GetNewContext())
            {
                var legality = DB.Legalities.Where(x => x.CardId == card.CardId).FirstOrDefault();
                if (legality == null) return true;
                if (string.IsNullOrEmpty(legality.LastUpdate)) return true;
                lastUpdate = DateTime.Parse(legality.LastUpdate);
            }
            if (lastUpdate < DateTime.Now.AddDays(-7)) return true;
            else return false;
        }
        private static bool IsRelatedCardsOutdated(MagicCard card)
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
        private static bool IsCardValuesOutdated(MagicCardVariant card)
        {
            if (string.IsNullOrEmpty(card.LastUpdate)) return true;
            DateTime lastUpdate = DateTime.Parse(card.LastUpdate);
            if (lastUpdate < DateTime.Now.AddDays(-1)) return true;
            else return false;
        }

        private static async Task SavePrice(MagicCardVariant cardVariant, Price price, int edhRank)
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
            catch (Exception e) { MessageBoxHelper.ShowError("SavePrice", e); }
        }
        private static async Task SaveLegality(MagicCard card, Dictionary<string, string> legalityDico)
        {
            try
            {
                List<Legality> legal = new();
                foreach (var l in legalityDico)
                {
                    legal.Add(new Legality()
                    {
                        Format = l.Key,
                        IsLegal = l.Value,
                        CardId = card.CardId,
                        LastUpdate = DateTime.Now.ToShortDateString(),
                    });
                }
                using var DB = App.DB.GetNewContext();
                {
                    DB.Legalities.AddRange(legal);
                    await DB.SaveChangesAsync();
                }
            }
            catch (Exception e) { MessageBoxHelper.ShowError("SaveLegality", e); }
        }
        private static async Task SaveRulings(MagicCard card, ResultList<Rule> rulings)
        {
            try
            {
                List<Rule> rule = new();
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
            catch (Exception e) { MessageBoxHelper.ShowError("SaveLegality", e); }
        }
        private static async Task SaveRelatedCards(MagicCard card, List<CardCardRelation> rels)
        {
            try
            {
                using var DB = App.DB.GetNewContext();
                await DB.CardRelations.AddRangeAsync(rels);
                DB.Entry(card).State = EntityState.Unchanged;
                await DB.SaveChangesAsync();
            }
            catch (Exception e) { MessageBoxHelper.ShowError("SaveRelated", e); }
        }

        private static async Task DestroyLegalitiesRecords(MagicCard localCard)
        {
            try
            {
                using var DB = App.DB.GetNewContext();
                IEnumerable<Legality> existingValues = DB.Legalities.Where(x => x.CardId == localCard.CardId);
                if (existingValues.Any())
                {
                    DB.Legalities.RemoveRange(existingValues);
                    await DB.SaveChangesAsync();
                }
            }
            catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }
        }
        private static async Task DestroyRelatedsRecords(MagicCard localCard)
        {
            try
            {
                using var DB = App.DB.GetNewContext();
                List<CardCardRelation> existingValues = await DB.CardRelations.Where(x => x.Card1Id == localCard.CardId).ToListAsync();
                if (existingValues == null || existingValues.Count == 0) return;
                DB.CardRelations.RemoveRange(existingValues);
                await DB.SaveChangesAsync();
            }
            catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }
        }

        private static async Task DestroyRulings(MagicCard localCard)
        {
            try
            {
                using var DB = App.DB.GetNewContext();
                List<Rule> existingValues = await DB.CardRules.Where(x => x.CardId == localCard.CardId).ToListAsync();
                if (existingValues == null || existingValues.Count == 0) return;
                DB.CardRules.RemoveRange(existingValues);
                await DB.SaveChangesAsync();
            }
            catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }
        }

        #endregion

    }

}