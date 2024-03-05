using MageekCore.Data;
using MageekCore.Data.Collection;
using MageekCore.Data.Collection.Entities;
using MageekCore.Data.Mtg;
using MageekCore.Data.Mtg.Entities;
using Microsoft.EntityFrameworkCore;
using PlaziatTools;
using ScryfallApi.Client.Models;
using System.Text.Json;

namespace MageekCore
{

    internal class ScryManager
    {

        private MtgDbManager mtg;
        private CollectionDbManager collec;

        public ScryManager(MtgDbManager mtg, CollectionDbManager collec)
        {
            this.mtg = mtg;
            this.collec = collec;
        }

        public async Task<List<CardCardRelation>> FindCard_Related(string uuid, string originalarchetype)
        {
            List<CardCardRelation> outputCards = new();
            try
            {
                if (string.IsNullOrEmpty(uuid)) return outputCards;
                var scryCard = await GetScryfallCard(uuid);
                if (scryCard == null) return outputCards;
                if (scryCard.AllParts == null) return outputCards;
                foreach (var part in scryCard.AllParts)
                {
                    part.TryGetValue("component", out string component);
                    part.TryGetValue("name", out string archetype);
                    if (!string.IsNullOrEmpty(component) && !string.IsNullOrEmpty(archetype))
                    {
                        if (archetype != originalarchetype)
                        {
                            CardCardRelationRole? role = null;
                            switch (component)
                            {
                                case "token": role = CardCardRelationRole.token; break;
                                case "meld_part": role = CardCardRelationRole.meld_part; break;
                                case "meld_result": role = CardCardRelationRole.meld_result; break;
                                case "combo_piece": role = CardCardRelationRole.combo_piece; break;
                            }
                            if (role.HasValue)
                            {
                                Cards cRes = null;
                                Tokens tRes = null;
                                if (role.Value == CardCardRelationRole.token)
                                {
                                    tRes = await FindToken(archetype);
                                }
                                else
                                {
                                    using MtgDbContext DB = await mtg.GetContext();
                                    cRes = await DB.cards
                                        .Where(x => x.Name == archetype)
                                        .FirstOrDefaultAsync();
                                }
                                outputCards.Add(new CardCardRelation()
                                {
                                    Role = role.Value,
                                    Card = cRes,
                                    Token = tRes
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e) { Logger.Log(e); }
            return outputCards;
        }
        public async Task<Tokens> FindToken(string name)
        {
            Logger.Log("");
            using MtgDbContext DB = await mtg.GetContext();
            return await DB.tokens
                .Where(x => x.Name == name)
                .FirstOrDefaultAsync();
        }


        /// <summary>
        /// This will disappear when using mtgsqlive data,
        /// get card data from scryfall from a card uuid
        /// </summary>
        /// <param name="cardUuid"></param>
        /// <returns>A scryfall card</returns>
        public async Task<Card?> GetScryfallCard(string cardUuid)
        {
            Logger.Log("");
            try
            {
                using MtgDbContext DB2 = await mtg.GetContext();
                var v = DB2.cardIdentifiers.Where(x => x.Uuid == cardUuid).FirstOrDefault();
                if (v == null) return null;
                string? scryfallId = v.ScryfallId;
                if (scryfallId == null) return null;
                Thread.Sleep(150);
                string json_data = await HttpUtils.Get("https://api.scryfall.com/cards/" + scryfallId);
                Card scryfallCard = JsonSerializer.Deserialize<Card>(json_data);
                return scryfallCard;
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return null;
            }
        }

        public async Task DownloadImage(string cardUuid, CardImageFormat type, string localFileName)
        {
            var scryData = await GetScryfallCard(cardUuid);
            var httpClient = new HttpClient();
            var uri = scryData.ImageUris[type.ToString()];
            using var stream = await httpClient.GetStreamAsync(uri);
            using var fileStream = new FileStream(localFileName, FileMode.Create);
            await stream.CopyToAsync(fileStream);
        }

        public async Task<PriceLine> EstimateCardPrice(string cardUuid)
        {
            using CollectionDbContext DB = await collec.GetContext();
            PriceLine? price = DB.PriceLine.Where(x => x.CardUuid == cardUuid).FirstOrDefault();
            if (price != null)
            {
                DateTime lastUpdate = DateTime.Parse(price.LastUpdate);
                if (lastUpdate < DateTime.Now.AddDays(-3)) return price;
                else price.LastUpdate = DateTime.Now.ToString();
            }
            Card? scryfallCard = await GetScryfallCard(cardUuid);
            if (scryfallCard == null) return null;
            if (price == null)
            {
                price = new PriceLine()
                {
                    CardUuid = cardUuid,
                    LastUpdate = DateTime.Now.ToString(),
                    PriceEur = scryfallCard.Prices.Eur,
                    PriceUsd = scryfallCard.Prices.Usd,
                    EdhrecScore = scryfallCard.EdhrecRank
                };
                DB.PriceLine.Add(price);
                await DB.SaveChangesAsync();
            }
            else
            {
                price.CardUuid = cardUuid;
                price.LastUpdate = DateTime.Now.ToString();
                price.PriceEur = scryfallCard.Prices.Eur;
                price.PriceUsd = scryfallCard.Prices.Usd;
                price.EdhrecScore = scryfallCard.EdhrecRank;
                DB.Entry(price).State = EntityState.Modified;
                await DB.SaveChangesAsync();
            }
            return price;
        }

        public async Task<ResultList<Set>> GetSetsJson()
        {
            string json_data = await HttpUtils.Get("https://api.scryfall.com/sets/");
            var sets = JsonSerializer.Deserialize<ResultList<Set>>(json_data);
            return sets;
        }
    }

}
