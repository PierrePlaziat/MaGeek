using MaGeek.AppData.Entities;
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

namespace MaGeek.AppBusiness
{

    public static class MageekApi
    {

        const int DelayApi = 150;

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
            }
            catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }
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
                        string cardId;
                        string cardName;
                        string relationType;
                        part.TryGetValue("id", out cardId);
                        part.TryGetValue("name", out cardName);
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

        #endregion

    }

}