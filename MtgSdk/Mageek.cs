using Microsoft.EntityFrameworkCore;
using MageekSdk.Collection.Entities;
using MageekSdk.Collection;
using MageekSdk.MtgSqlive.Entities;
using MageekSdk.MtgSqlive;
using System.Text;
using System.Text.Json;
using MaGeek.Framework.Data;
using ScryfallApi.Client.Models;
using MageekSdk;
using MaGeek.Framework.Extensions;

namespace MtgSqliveSdk
{

    public static class Mageek
    {

        public static Task Initialize()
        {
            throw new NotImplementedException();
        }

        #region Cards

        /// <summary>
        /// Get all card variants from an archetypal card name
        /// </summary>
        /// <param name="archetypeId"></param>
        /// <returns>a list of uuid</returns>
        public static async Task<List<string>> FindCard_Variants(string archetypeId)
        {
            using CollectionDbContext DB = await CollectionSdk.GetContext();
            return await DB.CardArchetypes
                .Where(x => x.ArchetypeId == archetypeId)
                .Select(p => p.CardUuid)
                .ToListAsync();
        }

        /// <summary>
        /// Get an archetypal card id from an card variant uuid
        /// </summary>
        /// <param name="cardUuid"></param>
        /// <returns>a single archetype id</returns>
        public static async Task<string> FindCard_Archetype(string cardUuid)
        {
            using CollectionDbContext DB = await CollectionSdk.GetContext();
            return await DB.CardArchetypes
                .Where(x => x.CardUuid == cardUuid)
                .Select(p => p.ArchetypeId)
                .FirstOrDefaultAsync();

        }

        public static async Task<ArchetypeCard> GetCardRef(string cardUuid)
        {
            using CollectionDbContext DB = await CollectionSdk.GetContext();
            return await DB.CardArchetypes
                .Where(x => x.CardUuid == cardUuid)
                .FirstOrDefaultAsync();

        }

        /// <summary>
        /// Add a card to the collection
        /// </summary>
        /// <param name="cardUuid"></param>
        public static async Task CollectedCard_Add(string cardUuid)
        {
            if (string.IsNullOrEmpty(cardUuid)) return;
            using CollectionDbContext DB = await CollectionSdk.GetContext();
            var collectedCard = await DB.CollectedCards.Where(x => x.CardUuid == cardUuid).FirstOrDefaultAsync();
            if (collectedCard == null)
            {
                // Create
                DB.CollectedCards.Add(new CollectedCard()
                {
                    CardUuid = cardUuid,
                    Collected = 1
                });
            }
            else
            {
                // Update
                collectedCard.Collected++;
                DB.Entry(collectedCard).State = EntityState.Modified;
            }
            await DB.SaveChangesAsync();
        }

        /// <summary>
        /// Remove a card from the collection
        /// </summary>
        /// <param name="cardUuid"></param>
        public static async Task CollectedCard_Remove(string cardUuid)
        {
            if (string.IsNullOrEmpty(cardUuid)) return;
            using CollectionDbContext DB = await CollectionSdk.GetContext();

            var collectedCard = await DB.CollectedCards.Where(x => x.CardUuid == cardUuid).FirstOrDefaultAsync();
            if (collectedCard != null && collectedCard.Collected > 0) 
            {
                // Update
                collectedCard.Collected--;
                DB.Entry(collectedCard).State = EntityState.Modified;
                await DB.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Counts how many cards collected variably
        /// </summary>
        /// <param name="cardUuid"></param>
        /// <param name="onlyThisVariant">set to false if you want to perform archetypal search from this card variant</param>
        /// <returns>The count</returns>
        public static async Task<int> CollectedCard_HowMany(string cardUuid, bool onlyThisVariant = true)
        {
            if (string.IsNullOrEmpty(cardUuid)) return 0;
            using CollectionDbContext DB = await CollectionSdk.GetContext();
            if (onlyThisVariant)
            {
                CollectedCard? collectedCard = await DB.CollectedCards.Where(x => x.CardUuid == cardUuid).FirstOrDefaultAsync();
                return collectedCard != null ? collectedCard.Collected : 0;
            }
            else
            {
                string archetypeId = DB.CardArchetypes.Where(x => x.CardUuid == cardUuid).First().ArchetypeId;
                return await CollectedCard_HowManyArchetypal(archetypeId);
            }
        }

        /// <summary>
        /// Counts how many cards collected archetypaly
        /// </summary>
        /// <param name="archetypeId"></param>
        /// <returns>The count</returns>
        public static async Task<int> CollectedCard_HowManyArchetypal(string archetypeId)
        {
            if (string.IsNullOrEmpty(archetypeId)) return 0;
            int count = 0;
            using CollectionDbContext DB = await CollectionSdk.GetContext();
            List<string> uuids = await DB.CardArchetypes.Where(x => x.ArchetypeId == archetypeId).Select(p=>p.CardUuid).ToListAsync();
            foreach (string uuid in uuids) count += await CollectedCard_HowMany(uuid);
            return count;
        }

        /// <summary>
        /// Determine favorite card variant for a card archetype
        /// </summary>
        /// <param name="archetypeId"></param>
        /// <param name="cardUuid"></param>
        public static async Task SetFav(string archetypeId, string cardUuid)
        {
            if (string.IsNullOrEmpty(archetypeId)) return;
            using CollectionDbContext DB = await CollectionSdk.GetContext();
            FavVariant? favCard = await DB.FavCards.Where(x => x.ArchetypeId == archetypeId).FirstOrDefaultAsync();
            if (favCard == null)
            {
                // Create
                DB.FavCards.Add(new FavVariant()
                {
                    ArchetypeId = archetypeId,
                    FavUuid = cardUuid
                });
            }
            else
            {
                // Update
                favCard.FavUuid = cardUuid;
                DB.Entry(favCard).State = EntityState.Modified;
            }
            await DB.SaveChangesAsync();
        }

        /// <summary>
        /// Get archetype name in a precise language
        /// </summary>
        /// <param name="archetypeId"></param>
        /// <param name="lang"></param>
        /// <returns>Traducted name of english one if not found</returns>
        public static async Task<string> GetTraduction(string archetypeId, string lang)
        {
            string foreignName = "";
            try
            {
                using CollectionDbContext DB = await CollectionSdk.GetContext();
                var t = await DB.CardTraductions.Where(x => x.CardUuid == archetypeId && x.Language == lang).FirstOrDefaultAsync();
                if (t != null) foreignName = t.Traduction;
            }
            catch (Exception e) { Console.WriteLine("Mageek : GetTraduction > error : " + e.Message); }
            if (string.IsNullOrEmpty(foreignName)) foreignName = archetypeId;
            return foreignName;
        }

        /// <summary>
        /// Get all traducted infos of the card
        /// </summary>
        /// <param name="cardUuid"></param>
        /// <param name="lang"></param>
        /// <returns>The data if any</returns>
        public static async Task<CardForeignData> GetTraductedData(string cardUuid, string lang)
        {
            try
            {
                using MtgSqliveDbContext DB = await MageekSdk.MtgSqlive.MtgSqliveSdk.GetContext();
                {
                    CardForeignData? cardForeignData = await DB.CardForeignData.Where(x => x.Uuid == cardUuid && x.Language == lang).FirstOrDefaultAsync();
                    return cardForeignData;
                }
            }
            catch (Exception e) 
            {
                Console.WriteLine("Mageek : GetTraductedData > error : " + e.Message);
                return null;
            }
        }

        /// <summary>
        /// Find if a card has a type
        /// </summary>
        /// <param name="cardUuid"></param>
        /// <param name="typeFilter"></param>
        /// <returns>true if it has it</returns>
        public static async Task<bool> CardHasType(string cardUuid, string typeFilter)
        {
            using MtgSqliveDbContext DB = await MageekSdk.MtgSqlive.MtgSqliveSdk.GetContext();
            string type = await DB.Cards.Where(x => x.Uuid == cardUuid).Select(x=>x.Type).FirstOrDefaultAsync();
            return type.Contains(typeFilter);
        }

        /// <summary>
        /// Estimate the price of a card
        /// </summary>
        /// <param name="v"></param>
        /// <param name="currency"></param>
        /// <returns>The estimation</returns>
        public static async Task<PriceLine> EstimateCardPrice(string cardUuid)
        {
            using CollectionDbContext DB = await CollectionSdk.GetContext();
            PriceLine? price = DB.Prices.Where(x => x.cardUuid == cardUuid).FirstOrDefault();
            if (price != null)
            {
                DateTime lastUpdate = DateTime.Parse(price.LastUpdate);
                if (lastUpdate < DateTime.Now.AddDays(-3)) return price;
                else price.LastUpdate = DateTime.Now.ToString();
            }
            Card? scryfallCard = await GetScryfallCard(cardUuid);
            if (scryfallCard == null) return null;
            if(price==null)
            {
                price = new PriceLine()
                {
                    cardUuid = cardUuid,
                    LastUpdate = DateTime.Now.ToString(),
                    PriceEur = scryfallCard.Prices.Eur,
                    PriceUsd = scryfallCard.Prices.Usd,
                    EdhrecScore = scryfallCard.EdhrecRank
                };
                DB.Prices.Add(price);
                await DB.SaveChangesAsync();
            }
            else
            {
                price.cardUuid = cardUuid;
                price.LastUpdate = DateTime.Now.ToString();
                price.PriceEur = scryfallCard.Prices.Eur;
                price.PriceUsd = scryfallCard.Prices.Usd;
                price.EdhrecScore = scryfallCard.EdhrecRank;
                DB.Entry(price).State = EntityState.Modified;
                await DB.SaveChangesAsync();
            }
            return price;
        }

        /// <summary>
        /// Get the devotion to a color of a card
        /// </summary>
        /// <param name="manaCost"></param>
        /// <param name="color"></param>
        /// <returns>The devotion to this color</returns>
        public static int Devotion(string manaCost, char color)
        {
            return manaCost.Length - manaCost.Replace(color.ToString(), "").Length;
        }

        private static async Task<Card?> GetScryfallCard(string cardUuid)
        {
            try
            {
                using MtgSqliveDbContext DB2 = await MageekSdk.MtgSqlive.MtgSqliveSdk.GetContext();
                string? scryfallId = DB2.CardIdentifiers.Where(x => x.Uuid == cardUuid).FirstOrDefault()?.ScryfallId;
                if (scryfallId == null) return null;
                Thread.Sleep(150);
                string json_data = await HttpUtils.Get("https://api.scryfall.com/cards/" + scryfallId);
                Card scryfallCard = JsonSerializer.Deserialize<Card>(json_data);
                return scryfallCard;
            }
            catch (Exception e)
            {
                Console.WriteLine("ScryfallApi : RetrieveCardValues > error : " + e.Message);
                return null;
            }
        }

        #region Tags

        /// <summary>
        /// List all existing tags
        /// </summary>
        /// <returns>List of distinct tags</returns>
        public static async Task<List<Tag>> GetTags()
        {
            List<Tag> tags = new();
            using CollectionDbContext DB = await CollectionSdk.GetContext();
            tags.Add(null);
            tags.AddRange(
                    DB.CardTags.GroupBy(x => x.TagContent).Select(x => x.First())
            );
            return tags;
        }

        ///// <summary>
        ///// List tags of a card
        ///// </summary>
        ///// <returns>List of tags</returns>
        //public static async Task<List<Tag>> GetTags(string archetypeId)
        //{
        //    List<Tag> tags = new();
        //    using CollectionDbContext DB = await CollectionSdk.GetContext();
        //    tags.Add(null);
        //    tags.AddRange(
        //            DB.CardTags.Where(x=>x.ArchetypeId==archetypeId).GroupBy(x => x.TagContent).Select(x => x.First())
        //    );
        //    return tags;
        //}

        /// <summary>
        /// Does this card have this tag
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="tagFilterSelected"></param>
        /// <returns>true if this card has this tag</returns>
        public static async Task<bool> HasTag(string cardId, string tagFilterSelected)
        {
            return (await GetTags(cardId)).Where(x => x.TagContent == tagFilterSelected).Any();
        }

        /// <summary>
        /// Add a tag to a card
        /// </summary>
        /// <param name="archetypeId"></param>
        /// <param name="text"></param>
        public static async Task TagCard(string archetypeId, string text)
        {
            using CollectionDbContext DB = await CollectionSdk.GetContext();
            DB.CardTags.Add(new Tag()
            {
                TagContent = text,
                ArchetypeId = archetypeId
            });
            await DB.SaveChangesAsync();
        }

        /// <summary>
        /// Remove a tag from a card
        /// </summary>
        /// <param name="cardTag"></param>
        public static async Task UnTagCard(Tag cardTag)
        {
            using CollectionDbContext DB = await CollectionSdk.GetContext();
            DB.CardTags.Remove(cardTag);
            await DB.SaveChangesAsync();
        }

        /// <summary>
        /// Find if this card has tags
        /// </summary>
        /// <param name="archetypeId"></param>
        /// <returns>List of tags</returns>
        public static async Task<List<Tag>> GetTags(string archetypeId)
        {
            List<Tag> tags = new();
            using CollectionDbContext DB = await CollectionSdk.GetContext();
            tags.AddRange(DB.CardTags.Where(x => x.ArchetypeId == archetypeId));
            return tags;
        }

        #endregion

        #endregion

        #region Decks

        /// <summary>
        /// Get decks registered
        /// </summary>
        /// <returns>A list containing the decks</returns>
        public static async Task<List<Deck>> GetDecks()
        {
            List<Deck> decks = new();
            try
            {
                using CollectionDbContext DB = await CollectionSdk.GetContext();
                decks = await DB.Decks.ToListAsync();
            }
            catch (Exception e) { Console.WriteLine("Mageek : GetDecks > error : " + e.Message); }
            return decks;
        }

        /// <summary>
        /// Get a deck by its id
        /// </summary>
        /// <param name="deckId"></param>
        /// <returns>The found deck or null</returns>
        public async static Task<Deck> GetDeck(int deckId)
        {
            try
            {
                using CollectionDbContext DB = await CollectionSdk.GetContext();
                Deck deck = await DB.Decks.FirstOrDefaultAsync();
                return deck;
            }
            catch (Exception e) 
            { 
                Console.WriteLine("Mageek : GetDecks > error : " + e.Message);
                return null;
            }
        }

        /// <summary>
        /// Gets deck cards
        /// </summary>
        /// <param name="deckId"></param>
        /// <returns>A list of deck-card relations</returns>
        public static async Task<List<DeckCard>> GetDeckContent(int deckId)
        {
            using CollectionDbContext DB = await CollectionSdk.GetContext();
            return await DB.DeckCards.Where(x => x.DeckId == deckId).ToListAsync();
        }

        /// <summary>
        /// Creates an empty deck
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <returns>a reference to the deck</returns>
        public static async Task<Deck> CreateDeck_Empty(string title, string description)
        {
            if (string.IsNullOrEmpty(title)) return null;
            try
            {
                using CollectionDbContext DB = await CollectionSdk.GetContext();
                Deck deck = new()
                {
                    Title = title,
                    CardCount = 0,
                    DeckColors = "",
                    Description = description
                };
                DB.Decks.Add(deck);
                await DB.SaveChangesAsync();
                return deck;
            }
            catch (Exception e) 
            {
                Console.WriteLine("Mageek : CreateDeck_Empty > error : " + e.Message);
                return null;
            }
        }

        /// <summary>
        /// Creates a filled deck
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="deckLines"></param>
        /// <returns>A list of messages, empty if everything went well</returns>
        public static async Task<List<string>> CreateDeck_Contructed(string title, string description, List<DeckLine> deckLines)
        {
            List<string> messages = new();
            Deck deck = await CreateDeck_Empty(title, description);
            if (deck == null)
            {
                messages.Add("[Error]Couldnt create the deck.");
                return messages;
            }
            else
            {
                try
                {
                    using (CollectionDbContext DB = await CollectionSdk.GetContext())
                    {
                        foreach (DeckLine deckLine in deckLines)
                        {
                            if (DB.CardArchetypes.Where(x => x.CardUuid == deckLine.Uuid).Any())
                            {
                                DB.DeckCards.Add(
                                    new DeckCard() {
                                        DeckId = deck.DeckId,
                                        CardUuid = deckLine.Uuid,
                                        Quantity = deckLine.Quantity,
                                        RelationType = deckLine.Relation
                                    }
                                );
                                deck.CardCount += deckLine.Quantity;
                            }
                            else
                            {
                                messages.Add("[CardNotFoud]" + deckLine.Uuid);
                            }
                        }

                        await DB.SaveChangesAsync();
                    }
                }
                catch (Exception e)
                {
                    messages.Add("[error]" + e.Message);
                    Console.WriteLine("Mageek : CreateDeck_Contructed > error : " + e.Message);
                }
            }
            return messages;
        }
        
        /// <summary>
        /// Data to aggregate to represent a constructed deck
        /// </summary>
        public struct DeckLine
        {
            public string Uuid;
            public int Quantity;
            public int Relation;
        }

        /// <summary>
        /// Rename a deck
        /// </summary>
        /// <param name="deck"></param>
        /// <param name="title"></param>
        public static async Task RenameDeck(int deckId, string title)
        {
            if (string.IsNullOrEmpty(title)) return;
            using CollectionDbContext DB = await CollectionSdk.GetContext();
            var deck = await DB.Decks.Where(x => x.DeckId == deckId).FirstOrDefaultAsync();
            if (deck!=null)
            {
                deck.Title = title;
                DB.Entry(deck).State = EntityState.Modified;
                await DB.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Duplicate a deck
        /// </summary>
        /// <param name="deckToCopy"></param>
        public static async Task DuplicateDeck(Deck deckToCopy)
        {
            if (deckToCopy == null) return;
            var newDeck = await CreateDeck_Empty(
                deckToCopy.Title + " - Copy",
                deckToCopy.Description);
            if(newDeck == null) return;
            
            using CollectionDbContext DB = await CollectionSdk.GetContext();
            foreach (DeckCard relation in DB.DeckCards.Where(x=>x.DeckId==deckToCopy.DeckId))
            {
                DB.DeckCards.Add(
                    new DeckCard()
                    {
                        CardUuid = relation.CardUuid,
                        DeckId = newDeck.DeckId,
                        Quantity = relation.Quantity,
                        RelationType = relation.RelationType
                    }
                );
            }
            newDeck.CardCount = deckToCopy.CardCount;
            newDeck.DeckColors = deckToCopy.DeckColors;
            DB.Entry(newDeck).State = EntityState.Modified;
            await DB.SaveChangesAsync();
        }

        /// <summary>
        /// Exports a txt list from a registered deck
        /// format: 
        /// X{SetCode} CardName
        /// </summary>
        /// <param name="deckId"></param>
        /// <param name="withSetCode"></param>
        /// <returns>the formated decklist</returns>
        public static async Task<string> DeckToTxt(int deckId, bool withSetCode = false)
        {
            using CollectionDbContext collection = await CollectionSdk.GetContext();
            using MtgSqliveDbContext cardInfos = await MageekSdk.MtgSqlive.MtgSqliveSdk.GetContext();
            var deck = await collection.Decks.Where(x => x.DeckId == deckId).FirstOrDefaultAsync();
            if (deck == null) return "";

            StringBuilder result = new();
            result.AppendLine(deck.Title);
            result.AppendLine(deck.Description);
            result.AppendLine();
            var cardRelations = await GetDeckContent(deck.DeckId);
            int lastRelationType = -1;
            foreach (DeckCard cardRelation in cardRelations
                .Where(x=>x.RelationType==0)
                .OrderBy(x => x.RelationType)
                .ThenBy(x=>x.RelationType==1))
            {
                if(lastRelationType!=cardRelation.RelationType)
                {
                    lastRelationType = cardRelation.RelationType;
                    switch(lastRelationType)
                    {
                        case 0: result.AppendLine("Content:"); break;
                        case 1: result.AppendLine("Commandment:"); break;
                        case 2: result.AppendLine("Side:"); break;
                    }
                }
                Cards? card = await cardInfos.Cards.Where(x => x.Uuid == cardRelation.CardUuid).FirstOrDefaultAsync();
                if(card!=null)
                {
                    result.Append(cardRelation.Quantity);
                    if (withSetCode)
                    {
                        result.Append("{");
                        result.Append(card.SetCode);
                        result.Append("}");
                    }
                    result.Append(" ");
                    result.Append(card.Name);
                    result.AppendLine();
                }
                else result.AppendLine("???");
            }
            return result.ToString();
        }

        /// <summary>
        /// Delete a deck
        /// </summary>
        /// <param name="deck"></param>
        public static async Task DeleteDeck(Deck deck)
        {
            using CollectionDbContext DB = await CollectionSdk.GetContext();
            DB.Decks.Remove(deck);
            await DB.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes some decks
        /// </summary>
        /// <param name="decks"></param>
        public static async Task DeleteDecks(List<Deck> decks)
        {
            using CollectionDbContext DB = await CollectionSdk.GetContext();
            DB.Decks.RemoveRange(decks);
            await DB.SaveChangesAsync();
        }

        /// <summary>
        /// Add a card to a deck without knowing which set
        /// </summary>
        /// <param name="archetypeId"></param>
        /// <param name="deck"></param>
        /// <param name="qty"></param>
        /// <param name="relation"></param>
        public static async Task AddCardToDeck_WithoutSet(string archetypeId, Deck deck, int qty, int relation = 0)
        {
            using CollectionDbContext DB = await CollectionSdk.GetContext();
            List<string> cardUuids = await FindCard_Variants(archetypeId);
            if (cardUuids.Count>0) await AddCardToDeck(cardUuids[0], deck, qty, relation);
        }

        /// <summary>
        /// Add a card to a deck knowing which set
        /// </summary>
        /// <param name="cardUuid"></param>
        /// <param name="deck"></param>
        /// <param name="qty"></param>
        /// <param name="relation"></param>
        /// <returns></returns>
        public static async Task AddCardToDeck(string cardUuid, Deck deck, int qty, int relation = 0)
        {
            if (string.IsNullOrEmpty(cardUuid) || deck == null) return;
            try
            {
                using (CollectionDbContext DB = await CollectionSdk.GetContext())
                {
                    var cardRelation = await DB.DeckCards.Where(x => x.CardUuid == cardUuid).FirstOrDefaultAsync();
                    if (cardRelation == null)
                    {
                        cardRelation = new DeckCard()
                        {
                            CardUuid = cardUuid,
                            DeckId = deck.DeckId,
                            Quantity = qty,
                            RelationType = relation
                        };
                        DB.Entry(cardRelation).State = EntityState.Added;
                    }
                    else
                    {
                        cardRelation.Quantity += qty;
                        DB.Entry(cardRelation).State = EntityState.Modified;
                    }
                    deck.CardCount += qty;
                    DB.Entry(deck).State = EntityState.Modified;
                    await DB.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Mageek : AddCardToDeck > error : " + e.Message);
            }
        }

        /// <summary>
        /// Remove card from a deck
        /// </summary>
        /// <param name="cardUuid"></param>
        /// <param name="deck"></param>
        /// <param name="qty"></param>
        public static async Task RemoveCardFromDeck(string cardUuid, Deck deck, int qty = 1)
        {
            if (string.IsNullOrEmpty(cardUuid) || deck == null) return;
            try
            {
                using (CollectionDbContext DB = await CollectionSdk.GetContext())
                {
                    var cardRelation = await DB.DeckCards.Where(x => x.CardUuid == cardUuid).FirstOrDefaultAsync();
                    if (cardRelation == null) return;
                    cardRelation.Quantity -= qty;
                    if (cardRelation.Quantity <= 0) DB.Entry(cardRelation).State = EntityState.Deleted;
                    else DB.Entry(cardRelation).State = EntityState.Modified;
                    deck.CardCount -= qty;
                    DB.Entry(deck).State = EntityState.Modified;
                    await DB.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Mageek : RemoveCardFromDeck > error : " + e.Message);
            }
        }

        /// <summary>
        /// Change the relation type of this card to this deck
        /// </summary>
        /// <param name="relation"></param>
        /// <param name="type"></param>
        public static async Task ChangeDeckRelationType(DeckCard relation, int type)
        {
            using (CollectionDbContext DB = await CollectionSdk.GetContext())
            {
                relation.RelationType = type;
                DB.Entry(relation).State = EntityState.Modified;
                await DB.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Switch a card in a deck
        /// </summary>
        /// <param name="cardDeckRelation"></param>
        /// <param name="cardUuid"></param>
        public static async Task SwitchCardInDeck(DeckCard cardDeckRelation, string cardUuid)
        {
            int qty = cardDeckRelation.Quantity;
            Deck deck = await GetDeck(cardDeckRelation.DeckId);
            int rel = cardDeckRelation.RelationType;
            await RemoveCardFromDeck(cardDeckRelation.CardUuid, deck, qty);
            await AddCardToDeck(cardUuid, deck, qty, rel);
        }

        /// <summary>
        /// Find deck color identity
        /// </summary>
        /// <param name="deckId"></param>
        /// <returns>Color identity string</returns>
        public static async Task<string> FindDeckColorIdentity(int deckId)
        {
            string retour = "";
            using CollectionDbContext DB = await CollectionSdk.GetContext();
            List<DeckCard> cards = await GetDeckContent(deckId);
            if (cards.Count > 0)
            {
                if (await DeckHasThisColorIdentity(cards, 'B')) retour += "B";
                if (await DeckHasThisColorIdentity(cards, 'W')) retour += "W";
                if (await DeckHasThisColorIdentity(cards, 'U')) retour += "U";
                if (await DeckHasThisColorIdentity(cards, 'G')) retour += "G";
                if (await DeckHasThisColorIdentity(cards, 'R')) retour += "R";
            }
            return retour;
        }
        private static async Task<bool> DeckHasThisColorIdentity(List<DeckCard> deck, char color)
        {
            using MtgSqliveDbContext DB = await MageekSdk.MtgSqlive.MtgSqliveSdk.GetContext();
            foreach (DeckCard card in deck)
            {
                var c = DB.Cards.Where(x => x.Uuid == card.CardUuid).FirstOrDefault();
                if (c != null)
                {
                    if (c.ColorIdentity.Contains(color)) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get the devotion to a color of a deck
        /// </summary>
        /// <param name="deckId"></param>
        /// <param name="color"></param>
        /// <returns>The devotion to this color</returns>
        public static async Task<int> Devotion(int deckId, char color)
        {
            using MtgSqliveDbContext DB = await MageekSdk.MtgSqlive.MtgSqliveSdk.GetContext();
            List<DeckCard> deckCards = await GetDeckContent(deckId);
            int devotion = 0;
            foreach (var card in deckCards)
            {
                var c = DB.Cards.Where(x => x.Uuid == card.CardUuid).FirstOrDefault();
                devotion += Devotion(c.ManaCost, color);
            }
            return devotion;
        }

        /// <summary>
        /// Estimate the price of a deck
        /// </summary>
        /// <param name="deckId"></param>
        /// <returns>The estimation</returns>
        public static async Task<Tuple<decimal, List<string>>> EstimateDeckPrice(int deckId, string currency)
        {
            decimal total = 0;
            List<string> missingList = new();
            using CollectionDbContext DB = await CollectionSdk.GetContext();
            List<DeckCard> deckCards = await GetDeckContent(deckId);
            foreach (var deckCard in deckCards)
            {
                var price = await EstimateCardPrice(deckCard.CardUuid);
                if (price != null)
                {
                    if (currency == "Eur") total += price.PriceEur.HasValue ? price.PriceEur.Value : 0;
                    if (currency == "Usd") total += price.PriceUsd.HasValue ? price.PriceUsd.Value : 0;
                    if (currency == "Edh") total += price.EdhrecScore;
                }
                else missingList.Add(deckCard.CardUuid);
            }
            return new Tuple<decimal, List<string>>(total, missingList);
        }

        #region counts

        /// <summary>
        /// Count total deck cards
        /// </summary>
        /// <param name="deckId"></param>
        /// <returns>the count regarding quantities</returns>
        public static async Task<int> Count_Total(int deckId)
        {
            int count = 0;
            using CollectionDbContext DB = await CollectionSdk.GetContext();
            foreach (DeckCard card in await GetDeckContent(deckId))
            {
                count += card.Quantity;
            }
            return count;
        }

        /// <summary>
        /// Existing types are:
        /// Land, Creature, Artifact, Enchantment, Planeswalker, Instant, Sorcery.
        /// </summary>
        /// <param name="deckId"></param>
        /// <param name="typeFilter"></param>
        /// <returns>The count</returns>
        public static async Task<int> Count_Typed(int deckId, string typeFilter)
        {
            int count = 0;
            using CollectionDbContext DB = await CollectionSdk.GetContext();
            var d = await GetDeckContent(deckId);
            foreach (DeckCard card in d)
            {
                if (await CardHasType(card.CardUuid, typeFilter)) count += card.Quantity;
            }
            return count;
        }

        /// <summary>
        /// Count by relation
        /// </summary>
        /// <param name="deckId"></param>
        /// <param name="typeFilter"></param>
        /// <returns>The count</returns>
        public static async Task<int> Count_Related(int deckId, int relationType)
        {
            int count = 0;
            using CollectionDbContext DB = await CollectionSdk.GetContext();
            var d = await GetDeckContent(deckId);
            foreach (DeckCard card in d)
            {
                if (card.RelationType == relationType) count += card.Quantity;
            }
            return count;
        }

        #endregion

        #region Types

        /// <summary>
        /// Count by relation
        /// </summary>
        /// <param name="deckId"></param>
        /// <param name="typeFilter"></param>
        /// <returns>The deck cards</returns>
        public static async Task<List<DeckCard>> GetDeckContent_Related(int deckId, int relationType)
        {
            List<DeckCard> rels = new List<DeckCard>();
            using CollectionDbContext DB = await CollectionSdk.GetContext();
            var d = await GetDeckContent(deckId);
            foreach (DeckCard card in d)
            {
                if (card.RelationType == relationType) rels.Add(card);
            }
            return rels;
        }

        /// <summary>
        /// Existing types are:
        /// Land, Creature, Artifact, Enchantment, Planeswalker, Instant, Sorcery.
        /// </summary>
        /// <param name="deckId"></param>
        /// <param name="typeFilter"></param>
        /// <returns>The deck cards</returns>
        public static async Task<List<DeckCard>> GetDeckContent_Typed(int deckId, string typeFilter)
        {
            List<DeckCard> rels = new List<DeckCard>();
            using CollectionDbContext DB = await CollectionSdk.GetContext();
            var d = await GetDeckContent(deckId);
            foreach (DeckCard card in d)
            {
                if (await CardHasType(card.CardUuid, typeFilter)) rels.Add(card);
            }
            return rels;
        }

        //public static async Task<List<DeckCard>> GetDeckContent_Cmc(float cmc)
        //{
        //    if (CurrentDeck == null || CurrentDeck.DeckCards == null) return null;
        //    List<DeckCard> cardRelations = new List<DeckCard>();
        //    foreach (var card in CurrentDeck.DeckCards.Where(x =>
        //            x.RelationType == 0
        //        && !x.Card.Card.Type.ToLower().Contains("land")
        //        && x.Card.Card.Cmc == 0
        //    ))
        //    {
        //        for (int i = 0; i < card.Quantity; i++)
        //        {
        //            cardRelations.Add(card);
        //        }
        //    }
        //    return cardRelations;
        //}

        #endregion

        #region Indicators

        /// <summary>
        /// Get the mana curve
        /// </summary>
        /// <param name="deckId"></param>
        /// <returns>ManaCurve</returns>
        public static async Task<int[]> GetManaCurve(int deckId)
        {
            using CollectionDbContext DB = await CollectionSdk.GetContext();
            using MtgSqliveDbContext DB2 = await MageekSdk.MtgSqlive.MtgSqliveSdk.GetContext();
            var content = await GetDeckContent(deckId);
            var manaCurve = new int[11] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            foreach (DeckCard c in content)
            {
                Cards card = await DB2.Cards.Where(x => x.Uuid == c.CardUuid).FirstOrDefaultAsync();
                int manacost = int.Parse(card.ManaCost);
                if (card != null && !card.Type.Contains("Land")) manaCurve[manacost <= 10 ? manacost : 10]++;
            }
            return manaCurve;
        }

        /// <summary>
        /// Get owned ratio in percents
        /// </summary>
        /// <param name="deckId"></param>
        /// <returns>percentage</returns>
        public static async Task<int> OwnedRatio(int deckId)
        {
            using CollectionDbContext DB = await CollectionSdk.GetContext();
            using MtgSqliveDbContext DB2 = await MageekSdk.MtgSqlive.MtgSqliveSdk.GetContext();
            var content = await GetDeckContent(deckId);
            int total = 0;
            int miss = 0;
            foreach (var v in content)
            {
                Cards card = await DB2.Cards.Where(x => x.Uuid == v.CardUuid).FirstOrDefaultAsync();
                if (!card.Type.Contains("Basic Land"))
                {
                    total += v.Quantity;
                    int got = await CollectedCard_HowMany(v.CardUuid, false);
                    int need = v.Quantity;
                    int diff = need - got;
                    if (diff > 0) miss += diff;
                }
            }
            if (total == 0) return 100;
            return 100 - miss * 100 / total;
        }

        /// <summary>
        /// Get txt list of missing cards
        /// </summary>
        /// <param name="deckId"></param>
        /// <returns>a text formated as : X cardName\n</returns>
        public static async Task<string> ListMissingCards(int deckId)
        {
            using CollectionDbContext DB = await CollectionSdk.GetContext();
            using MtgSqliveDbContext DB2 = await MageekSdk.MtgSqlive.MtgSqliveSdk.GetContext();
            var content = await GetDeckContent(deckId);
            string missList = "";
            foreach (var v in content)
            {
                Cards card = await DB2.Cards.Where(x => x.Uuid == v.CardUuid).FirstOrDefaultAsync();
                if (!card.Type.Contains("Basic Land"))
                {
                    int got = await CollectedCard_HowMany(v.CardUuid, false);
                    int need = v.Quantity;
                    int diff = need - got;
                    if (diff > 0) missList += diff + " " + card.Name + "\n";
                }
            }
            return missList;
        }

        #endregion

        #region Validities

        /// <summary>
        /// Get deck validity in a given format
        /// </summary>
        /// <param name="deck"></param>
        /// <param name="format"></param>
        /// <returns>"OK" or an error msg</returns>
        public static async Task<string> DeckValidity(Deck deck, string format)
        {
            if (deck == null) return "";
            int minCards = GetMinCardInFormat(format);
            int maxCards = GetMaxCardInFormat(format);
            if (deck.CardCount > maxCards) return "Maximum " + maxCards + " cards.";
            if (deck.CardCount < minCards) return "Minimum " + maxCards + " cards.";
            int maxOccurence = GetMaxOccurenceInFormat(format);

            using MtgSqliveDbContext DB = await MageekSdk.MtgSqlive.MtgSqliveSdk.GetContext();
            foreach (var v in await GetDeckContent(deck.DeckId))
            {
                Cards card = await DB.Cards.Where(x => x.Uuid == v.CardUuid).FirstOrDefaultAsync();
                if (!card.Type.Contains("Basic Land"))
                {
                    CardLegalities cardLegalities = await DB.CardLegalities.Where(x => x.Uuid == v.CardUuid).FirstOrDefaultAsync();
                    string legal = "";
                    switch (format)
                    {
                        case "Alchemy": legal = cardLegalities.Alchemy; break;
                        case "Brawl": legal = cardLegalities.Brawl; break;
                        case "Commander": legal = cardLegalities.Commander; break;
                        case "Duel": legal = cardLegalities.Duel; break;
                        case "Explorer": legal = cardLegalities.Explorer; break;
                        case "Future": legal = cardLegalities.Future; break;
                        case "Gladiator": legal = cardLegalities.Gladiator; break;
                        case "Historic": legal = cardLegalities.Historic; break;
                        case "Historicbrawl": legal = cardLegalities.Historicbrawl; break;
                        case "Legacy": legal = cardLegalities.Legacy; break;
                        case "Modern": legal = cardLegalities.Modern; break;
                        case "Oathbreaker": legal = cardLegalities.Oathbreaker; break;
                        case "Oldschool": legal = cardLegalities.Oldschool; break;
                        case "Pauper": legal = cardLegalities.Pauper; break;
                        case "Paupercommander": legal = cardLegalities.Paupercommander; break;
                        case "Penny": legal = cardLegalities.Penny; break;
                        case "Pioneer": legal = cardLegalities.Pioneer; break;
                        case "Predh": legal = cardLegalities.Predh; break;
                        case "Premodern": legal = cardLegalities.Premodern; break;
                        case "Standard": legal = cardLegalities.Standard; break;
                        case "Vintage": legal = cardLegalities.Vintage; break;
                    }
                    if (legal == null || legal == "Legal")
                    {
                        if (v.Quantity > maxOccurence) return "Too many " + card.Name + ".";
                    }
                    if (legal == "Restricted")
                    {
                        if (v.Quantity > 1) return card.Name + " restricted.";
                    }
                    if (legal == "Banned")
                    {
                        if (v.Quantity > maxOccurence) return card.Name + " banned.";
                    }
                }
            }
            return "OK";
        }

        private static int GetMinCardInFormat(string format)
        {
            switch (format)
            {
                case "Alchemy": return 60;
                case "Brawl": return 60;
                case "Commander": return 100;
                case "Duel": return 60;
                case "Explorer": return 60;
                case "Future": return 60;
                case "Gladiator": return 60;
                case "Historic": return 60;
                case "Historicbrawl": return 60;
                case "Legacy": return 60;
                case "Modern": return 60;
                case "Oathbreaker": return 60;
                case "Oldschool": return 60;
                case "Pauper": return 60;
                case "Paupercommander": return 100;
                case "Penny": return 60;
                case "Pioneer": return 60;
                case "Predh": return 60;
                case "Premodern": return 60;
                case "Standard": return 60;
                case "Vintage": return 60;
            }
            return 60;
        }

        private static int GetMaxCardInFormat(string format)
        {
            switch (format)
            {
                case "Commander": return 100;
                case "Paupercommander": return 100;
                case "Explorer": return 250;
                default: return -1;
            }
        }

        private static int GetMaxOccurenceInFormat(string format)
        {
            switch (format)
            {
                case "commander": return 1;
                default: return 4;
            }
        }

        #endregion

        #endregion

        #region Sets

        /// <summary>
        /// Get cards in a set
        /// </summary>
        /// <param name="setCode"></param>
        /// <returns>Uuid list of cards in set</returns>
        public async static Task<List<string>> GetCardsFromSet(string setCode)
        {
            List<string> cardUuids = new();
            if (string.IsNullOrEmpty(setCode))
            {
                try
                {
                    using MtgSqliveDbContext DB = await MageekSdk.MtgSqlive.MtgSqliveSdk.GetContext();
                    {
                        cardUuids = await DB.Cards.Where(x => x.SetCode == setCode)
                            .Select(p=>p.Uuid)
                            .ToListAsync();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Mageek : GetCardsFromSet > error : " + e.Message);
                }
            }
            return cardUuids;
        }

        /// <summary>
        /// How many cards collected in this set
        /// </summary>
        /// <param name="setCode"></param>
        /// <param name="strict">if set to false, the archetype from any set counts</param>
        /// <returns>the distinct count</returns>
        public static async Task<int> GetMtgSetCompletion(string setCode, bool strict)
        {
            int nb = 0;
            try
            {
                var cardUuids = await GetCardsFromSet(setCode);
                using (CollectionDbContext DB = await CollectionSdk.GetContext())
                {
                    foreach (var cardUuid in cardUuids)
                    {
                        if (await CollectedCard_HowMany(cardUuid, strict) > 0) nb++;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Mageek : GetMtgSetCompletion > error : " + e.Message);
            }
            return nb;
        }

        #endregion

        #region Collection

        /// <summary>
        /// Totality of cards including their quantity
        /// </summary>
        /// <returns></returns>
        public static async Task<int> GetTotal_Collected()
        {
            int total = 0;
            try
            {
                using CollectionDbContext DB = await CollectionSdk.GetContext();
                total = DB.CollectedCards.Sum(x => x.Collected);
            }
            catch (Exception e)
            {
                Console.WriteLine("Mageek : GetTotalOwned > error : " + e.Message);
            }
            return total;
        }

        /// <summary>
        /// Totality of cards variants but doesnt sur their quantity
        /// </summary>
        /// <returns></returns>
        public static async Task<int> GetTotal_CollectedDiff()
        {
            try
            {
                using CollectionDbContext DB = await CollectionSdk.GetContext();
                return DB.CollectedCards.Count();
            }
            catch (Exception e)
            {
                Console.WriteLine("Mageek : GetTotalDiffGot > error : " + e.Message);
                return 0;
            }
        }

        /// <summary>
        /// Totality of different archetypes
        /// </summary>
        /// <returns></returns>
        public static async Task<int> GetTotal_CollectedArchetype()
        {
            try
            {
                using CollectionDbContext DB = await CollectionSdk.GetContext();
                return DB.CollectedCards.Count(); //TODO this is wrong
            }
            catch (Exception e)
            {
                Console.WriteLine("Mageek : GetTotalDiffGot > error : " + e.Message);
                return 0;
            }
        }

        /// <summary>
        /// Totality of different existing card archetypes
        /// </summary>
        /// <returns></returns>
        public static async Task<int> GetTotal_ExistingArchetypes()
        {
            int total = 0;
            try
            {
                using CollectionDbContext DB = await CollectionSdk.GetContext();
                total = DB.CardArchetypes
                    .GroupBy(x => x.ArchetypeId)
                    .Select(grp => grp.First())
                    .Count();
            }
            catch (Exception e)
            {
                Console.WriteLine("Mageek : GetTotalDiffExist > error : " + e.Message);
            }
            return total;
        }

        /// <summary>
        /// Auto estimate collection
        /// </summary>
        /// <returns>Estimated price and a list of missing estimations</returns>
        public static async Task<Tuple<decimal, List<string>>> AutoEstimatePrices(string currency)
        {
            decimal total = 0;
            List<string> missingList = new();
            try
            {
                using CollectionDbContext DB = await CollectionSdk.GetContext();
                var allGot = await DB.CollectedCards.Where(x => x.Collected > 0).ToListAsync();
                foreach (CollectedCard collectedCard in allGot)
                {
                    var price = await EstimateCardPrice(collectedCard.CardUuid);
                    if (price!=null)
                    {
                        if (currency=="Eur") total += price.PriceEur.HasValue ? price.PriceEur.Value : 0;
                        if (currency=="Usd") total += price.PriceUsd.HasValue ? price.PriceUsd.Value : 0;
                        if (currency=="Edh") total += price.EdhrecScore;
                    }
                    else missingList.Add(collectedCard.CardUuid);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Mageek : AutoEstimatePrices > error : " + e.Message);
            }
            return new Tuple<decimal, List<string>>(total, missingList);
        }

        #endregion

        public async static Task<Uri> RetrieveImage(string cardUuid)
        {
            try
            {
                string localFileName = Path.Combine(Config.Path_IllustrationsFolder, cardUuid + ".png");
                if (!File.Exists(localFileName))
                {
                    using MtgSqliveDbContext DB = await MageekSdk.MtgSqlive.MtgSqliveSdk.GetContext();
                    var v =  await DB.CardIdentifiers.Where(x => x.Uuid == cardUuid).Select(x => x.ScryfallIllustrationId).FirstOrDefaultAsync();
                    var httpClient = new HttpClient();
                    using var stream = await httpClient.GetStreamAsync(v);
                    using var fileStream = new FileStream(localFileName, FileMode.Create);
                    await stream.CopyToAsync(fileStream);
                }
                return new("file://" + Path.GetFullPath(localFileName), UriKind.Absolute);
            }
            catch (Exception e)
            {
                Console.WriteLine("Mageek : RetrieveImage > error : " + e.Message);
                return null;
            }
        }

        public async static Task<string?> GetCardBack(string cardUuid)
        {
            using MtgSqliveDbContext DB = await MageekSdk.MtgSqlive.MtgSqliveSdk.GetContext();
            return await DB.Cards.Where(x=>x.Uuid==cardUuid).Select(x=>x.OtherFaceIds).FirstOrDefaultAsync();
        }

        public async static Task<List<Cards>> NormalSearch(string lang, string filterName)
        {
            List<Cards> retour = new List<Cards>();
            string lowerFilterName = filterName.ToLower();
            string normalizedFilterName = StringExtension.RemoveDiacritics(filterName).Replace('-', ' ').ToLower();
            using (CollectionDbContext DB = await CollectionSdk.GetContext())
            using (MtgSqliveDbContext DB2 = await MageekSdk.MtgSqlive.MtgSqliveSdk.GetContext())
            {
                if (!string.IsNullOrEmpty(filterName))
                {
                        // Search in VO
                        var voResults = await DB.CardArchetypes.Where(x => x.ArchetypeId.ToLower().Contains(lowerFilterName)).ToListAsync();
                        foreach (var vo in voResults) retour.AddRange(DB2.Cards.Where(x =>x.Uuid==vo.CardUuid));
                        // Search in foreign
                        var tradResults = await DB.CardTraductions.Where(x => x.Language == lang && x.NormalizedTraduction.Contains(normalizedFilterName)).ToListAsync();
                        foreach (var trad in tradResults) retour.AddRange(DB2.Cards.Where(x => x.Uuid == trad.CardUuid));
                }
                else retour.AddRange(await DB2.Cards.ToArrayAsync());
            }
            // Remove duplicata
            retour = retour.GroupBy(x => x.Name).Select(g => g.First()).ToList();
            return retour;
        }

        public async static Task<List<Cards>> AdvancedSearch(string lang, string filterName, string filterType, string filterKeyword, string filterText, string filterColor, string filterTag, bool onlyGot)
        {
            List<Cards> retour = new();

            using (CollectionDbContext DB = await CollectionSdk.GetContext())
            using (MtgSqliveDbContext DB2 = await MageekSdk.MtgSqlive.MtgSqliveSdk.GetContext())
            {
                if (onlyGot)
                {
                    //retour.AddRange(await DB.CardModels.Where(x => x.Got > 0).ToArrayAsync());
                }
                else
                {
                    retour.AddRange(await DB2.Cards.ToArrayAsync());
                }
            }

            if (!string.IsNullOrEmpty(filterType))
            {
                string type = filterType.ToLower();
                retour = retour.Where(x => x.Type.ToLower().Contains(type)).ToList();
            }

            if (!string.IsNullOrEmpty(filterKeyword))
            {
                string keyword = filterKeyword.ToLower();
                retour = retour.Where(x => x.Keywords.ToLower().Contains(keyword)).ToList();
            }

            if (!string.IsNullOrEmpty(filterText))
            {
                string text = filterText.ToLower();
                retour = retour.Where(x => x.Text.ToLower().Contains(filterKeyword.ToLower())).ToList();
            }

            if (filterColor != "_")
            {
                switch (filterColor)
                {
                    case "X": retour = retour.Where(x => string.IsNullOrEmpty(x.ColorIdentity)).ToList(); break;
                    case "W": retour = retour.Where(x => x.ColorIdentity == "W").ToList(); break;
                    case "B": retour = retour.Where(x => x.ColorIdentity == "B").ToList(); break;
                    case "U": retour = retour.Where(x => x.ColorIdentity == "U").ToList(); break;
                    case "G": retour = retour.Where(x => x.ColorIdentity == "G").ToList(); break;
                    case "R": retour = retour.Where(x => x.ColorIdentity == "R").ToList(); break;
                    case "GW": retour = retour.Where(x => x.ColorIdentity == "G,W").ToList(); break;
                    case "WU": retour = retour.Where(x => x.ColorIdentity == "U,W").ToList(); break;
                    case "BU": retour = retour.Where(x => x.ColorIdentity == "B,U").ToList(); break;
                    case "RB": retour = retour.Where(x => x.ColorIdentity == "B,R").ToList(); break;
                    case "GR": retour = retour.Where(x => x.ColorIdentity == "G,R").ToList(); break;
                    case "GU": retour = retour.Where(x => x.ColorIdentity == "G,U").ToList(); break;
                    case "WB": retour = retour.Where(x => x.ColorIdentity == "B,W").ToList(); break;
                    case "RU": retour = retour.Where(x => x.ColorIdentity == "R,U").ToList(); break;
                    case "GB": retour = retour.Where(x => x.ColorIdentity == "B,G").ToList(); break;
                    case "RW": retour = retour.Where(x => x.ColorIdentity == "R,W").ToList(); break;
                    case "GBW": retour = retour.Where(x => x.ColorIdentity == "B,G,W").ToList(); break;
                    case "GWU": retour = retour.Where(x => x.ColorIdentity == "G,U,W").ToList(); break;
                    case "WRU": retour = retour.Where(x => x.ColorIdentity == "R,U,W").ToList(); break;
                    case "GRW": retour = retour.Where(x => x.ColorIdentity == "G,R,W").ToList(); break;
                    case "WUB": retour = retour.Where(x => x.ColorIdentity == "B,U,W").ToList(); break;
                    case "GUR": retour = retour.Where(x => x.ColorIdentity == "G,R,U").ToList(); break;
                    case "GRB": retour = retour.Where(x => x.ColorIdentity == "B,G,R").ToList(); break;
                    case "RUB": retour = retour.Where(x => x.ColorIdentity == "B,R,U").ToList(); break;
                    case "BGU": retour = retour.Where(x => x.ColorIdentity == "B,G,U").ToList(); break;
                    case "RWB": retour = retour.Where(x => x.ColorIdentity == "B,R,W").ToList(); break;
                    case "BGUR": retour = retour.Where(x => x.ColorIdentity == "B,G,R,U").ToList(); break;
                    case "GURW": retour = retour.Where(x => x.ColorIdentity == "G,R,U,W").ToList(); break;
                    case "URWB": retour = retour.Where(x => x.ColorIdentity == "B,R,U,W").ToList(); break;
                    case "RWBG": retour = retour.Where(x => x.ColorIdentity == "B,G,R,W").ToList(); break;
                    case "WBGU": retour = retour.Where(x => x.ColorIdentity == "B,G,U,W").ToList(); break;
                    case "WBGUR": retour = retour.Where(x => x.ColorIdentity == "B,G,R,U,W").ToList(); break;
                }

            }

            if (filterTag != null)
            {
                var tagged = new List<Cards>();
                foreach (var card in retour)
                {
                    if (await Mageek.HasTag(card.Name, filterTag))
                    {
                        tagged.Add(card);
                    }
                }
                retour = new List<Cards>(tagged);
            }

            return retour;
        }

        public async static Task<List<Sets>> LoadSets()
        {
            using (MtgSqliveDbContext DB = await MageekSdk.MtgSqlive.MtgSqliveSdk.GetContext())
            {
                return DB.Sets.OrderBy(x => x.ReleaseDate).ToList();
            }
        }

        public async static Task<List<CardLegalities>> GetLegalities(ArchetypeCard selectedCard)
        {
            using (MtgSqliveDbContext DB = await MageekSdk.MtgSqlive.MtgSqliveSdk.GetContext())
            {
                return await DB.CardLegalities.Where(x => x.Uuid == selectedCard.CardUuid).ToListAsync();
            }
        }

        public async static Task<List<CardRulings>> GetRulings(ArchetypeCard selectedCard)
        {
            using (MtgSqliveDbContext DB = await MageekSdk.MtgSqlive.MtgSqliveSdk.GetContext())
            {
                return await DB.CardRulings.Where(x=>x.Uuid== selectedCard.CardUuid).ToListAsync();
            }
        }

    }

}
