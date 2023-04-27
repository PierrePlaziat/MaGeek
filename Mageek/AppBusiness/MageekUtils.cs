using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.IO;
using System.Threading;
using System.Net.Http;
using MaGeek.AppData.Entities;
using Plaziat.CommonWpf;
using Microsoft.EntityFrameworkCore;
using MaGeek.AppData;
using System.Text.Json;
using ScryfallApi.Client.Models;
using System.Reflection;

namespace MaGeek.AppBusiness
{

    /// <summary>
    /// All business logic
    /// Full static async functionnal style
    /// </summary>
    public static class MageekUtils
    {

        #region API

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
                    if (result==null) data = await HttpUtils.Get("https://api.scryfall.com/cards/search?order=cmc&q=e:" + setCode);
                    else data = await HttpUtils.Get(result.NextPage.ToString());
                    result = JsonSerializer.Deserialize<ResultList<Card>>(data);
                    cards.AddRange(result.Data);
                }
                while (result.HasMore);
            }
            catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }
            return cards;
        }

        public static async Task<List<Card>> RetrieveCard(string cardName, bool exactName, bool skipIfExists)
        {
            List<Card> cards = new();
            try
            {
                if (skipIfExists)
                {
                    using (var DB = App.Biz.DB.GetNewContext())
                    {
                        if (DB.Cards.Where(x => x.CardId == cardName).Any()) return new List<Card>();
                    }
                }
                cards = await RetrieveCard(cardName);
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
                    if (result == null) data = await HttpUtils.Get("https://api.scryfall.com/cards/search?order=cmc&q=" + cardName+ "+unique:prints");
                    else data = await HttpUtils.Get(result.NextPage.ToString());
                    result = JsonSerializer.Deserialize<ResultList<Card>>(data);
                    cards.AddRange(result.Data);
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

        public static async Task RecordCard(Card scryCard, bool Owned)
        {
            try
            {
                MagicCard localCard;
                MagicCardVariant localVariant;
                using (var DB = App.Biz.DB.GetNewContext())
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
                using var DB = App.Biz.DB.GetNewContext();
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
                using var DB = App.Biz.DB.GetNewContext();
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
                if(scryfallCard.AllParts != null)
                {
                    foreach (var part in scryfallCard.AllParts)
                    {
                        string cardId;
                        string cardName;
                        string relationType;
                        part.TryGetValue("id", out cardId);
                        part.TryGetValue("name", out cardName);
                        part.TryGetValue("component", out relationType);

                        if(cardName != card.CardId)
                        {
                            MagicCard relatedCard;
                            using (var DB = App.Biz.DB.GetNewContext())
                            {
                                relatedCard = await DB.Cards.Where(x => x.CardId == cardName)
                                        .FirstOrDefaultAsync();
                            }
                            if (relatedCard == null)
                            {
                                var retrieved = await HttpUtils.Get("https://api.scryfall.com/cards/" + cardId);
                                Card result = JsonSerializer.Deserialize<Card>(retrieved);
                                await RecordCard(result, false);
                                using (var DB = App.Biz.DB.GetNewContext())
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
            using (var DB = App.Biz.DB.GetNewContext())
            {
                var legality = DB.Legalities.Where(x => x.CardId == card.CardId).FirstOrDefault();
                if (legality==null) return true;
                if (string.IsNullOrEmpty(legality.LastUpdate)) return true;
                lastUpdate = DateTime.Parse(legality.LastUpdate);
            }
            if (lastUpdate < DateTime.Now.AddDays(-7)) return true;
            else return false;
        }
        private static bool IsRelatedCardsOutdated(MagicCard card)
        {
            DateTime lastUpdate;
            using (var DB = App.Biz.DB.GetNewContext())
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
                using var DB = App.Biz.DB.GetNewContext();
                if(price!=null)
                {
                    if (price.Eur != null) cardVariant.ValueEur = price.Eur.ToString();
                    if (price.Usd != null) cardVariant.ValueUsd = price.Usd.ToString();
                    cardVariant.EdhRecRank = edhRank;
                    cardVariant.LastUpdate = DateTime.Now.ToShortDateString();
                }
                DB.Entry(cardVariant).State=EntityState.Modified;
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
                using var DB = App.Biz.DB.GetNewContext();
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
                using var DB = App.Biz.DB.GetNewContext();
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
                using var DB = App.Biz.DB.GetNewContext();
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
                using var DB = App.Biz.DB.GetNewContext();
                List<CardCardRelation> existingValues = await DB.CardRelations.Where(x => x.Card1Id == localCard.CardId).ToListAsync();
                if (existingValues == null || existingValues.Count == 0) return;
                DB.CardRelations.RemoveRange(existingValues);
                await DB.SaveChangesAsync();
            }
            catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }
        }

        #endregion
        
        #endregion

        #region Collection

        #region Cards

        public static async Task<MagicCard> FindCardById(string cardId)
        {
            using var DB = App.Biz.DB.GetNewContext();
            return await DB.Cards.Where(x => x.CardId == cardId)
                            .Include(card => card.Traductions)
                            .Include(card => card.Variants)
                            .FirstOrDefaultAsync();
        }

        public static async Task GotCard_Add(MagicCardVariant selectedCard)
        {
            if (selectedCard == null) return;
            using var DB = App.Biz.DB.GetNewContext();
            var c = DB.CardVariants.Where(x => x.Id == selectedCard.Id).FirstOrDefault();
            c.Got++;
            DB.Entry(c).State = EntityState.Modified;
            await DB.SaveChangesAsync();
        }

        public static async Task GotCard_Remove(MagicCardVariant selectedCard)
        {
            if (selectedCard == null) return;
            using var DB = App.Biz.DB.GetNewContext();
            var c = DB.CardVariants.Where(x => x.Id == selectedCard.Id).FirstOrDefault();
            c.Got--;
            if (c.Got < 0) c.Got = 0;
            DB.Entry(c).State = EntityState.Modified;
            await DB.SaveChangesAsync();
        }

        public static async Task SetFav(MagicCard card, MagicCardVariant variant)
        {
            using var DB = App.Biz.DB.GetNewContext();
            card.FavouriteVariant = variant.Id;
            DB.Entry(card).State = EntityState.Modified;
            await DB.SaveChangesAsync();
        }

        public static async Task<BitmapImage> RetrieveImage(MagicCardVariant magicCardVariant, bool back=false, int nbTry=0)
        {
            BitmapImage img = null;
            try
            {
                var taskCompletion = new TaskCompletionSource<BitmapImage>();
                string localFileName = "";
                if (magicCardVariant.IsCustom == 0)
                {
                    if(back)
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
            catch (Exception e) {
                await Task.Run(() => {
                    Thread.Sleep(rnd.Next(10)*50);
                });
                if (nbTry<3) return await RetrieveImage(magicCardVariant, back, nbTry++);
            }
            return img;
        }

        // TODO call only once at card creation
        #region Devotions

        private static async Task<int> DevotionB(MagicCard card)
        {
            int devotion = 0;
            await Task.Run(() => {
                devotion = card.ManaCost != null ?
                card.ManaCost.Length - card.ManaCost.Replace("B", "").Length
                : 0;
            });
            return devotion;
        }
        private static async Task<int> DevotionW(MagicCard card)
        {
            int devotion = 0;
            await Task.Run(() => {
                devotion = card.ManaCost != null ?
                card.ManaCost.Length - card.ManaCost.Replace("W", "").Length
                : 0;
            });
            return devotion;
        }
        private static async Task<int> DevotionU(MagicCard card)
        {
            int devotion = 0;
            await Task.Run(() => {
                devotion = card.ManaCost != null ?
                card.ManaCost.Length - card.ManaCost.Replace("U", "").Length
                : 0;
            });
            return devotion;
        }
        private static async Task<int> DevotionG(MagicCard card)
        {
            int devotion = 0;
            await Task.Run(() => {
                devotion = card.ManaCost != null ?
                card.ManaCost.Length - card.ManaCost.Replace("G", "").Length
                : 0;
            });
            return devotion;
        }
        private static async Task<int> DevotionR(MagicCard card)
        {
            int devotion = 0;
            await Task.Run(() => {
                devotion = card.ManaCost != null ?
                card.ManaCost.Length - card.ManaCost.Replace("R", "").Length
                : 0;
            });
            return devotion;
        }

        #endregion

        #endregion

        #region Decks

        public static async Task AddEmptyDeck()
        {
            try
            {
                using var DB = App.Biz.DB.GetNewContext();
                string deckTitle = MessageBoxHelper.UserInputString("Please enter a title for this new deck", "");
                if (deckTitle == null) return;
                if (DB.Decks.Where(x => x.Title == deckTitle).Any())
                {
                    MessageBoxHelper.ShowMsg("There is already a deck with that name.");
                    return;
                }
                MagicDeck deck = new(deckTitle);
                DB.Decks.Add(deck);
                await DB.SaveChangesAsync();
                App.Events.RaiseUpdateDeckList();
                App.Events.RaiseDeckSelect(deck);
            }
            catch (Exception e) { MessageBoxHelper.ShowError("AddEmptyDeck", e); }
        }

        public static async Task AddDeck(List<ImportLine> importLines, string title)
        {
            try
            {
                using var DB = App.Biz.DB.GetNewContext();
                {
                    var deck = new MagicDeck(title);
                    DB.Decks.Add(deck);
                    await DB.SaveChangesAsync();
                    MagicCard card;
                    foreach (var cardOccurence in importLines)
                    {
                        card = DB.Cards.Where(x => x.CardId == cardOccurence.Name)
                                                 .Include(x => x.Variants)
                                                 .FirstOrDefault();
                        if (card != null)
                        {
                            MagicCardVariant variant = card.Variants[0];
                            var cardRelation = deck.CardRelations.Where(x => x.Card.Card.CardId == card.CardId).FirstOrDefault();
                            if (cardRelation == null)
                            {
                                cardRelation = new CardDeckRelation()
                                {
                                    Card = variant,
                                    CardId = variant.Id,
                                    Deck = deck,
                                    Quantity = cardOccurence.Quantity,
                                    RelationType = cardOccurence.Side ? 2 : 0
                                };
                                DB.Entry(cardRelation).State = EntityState.Added;
                                deck.CardRelations.Add(cardRelation);
                            }
                            else
                            {
                                cardRelation.Quantity += cardOccurence.Quantity;
                                DB.Entry(cardRelation).State = EntityState.Modified;
                            }
                            deck.CardCount += cardOccurence.Quantity;
                        }
                    }
                    DB.Entry(deck).State = EntityState.Modified;
                    await DB.SaveChangesAsync();
                }
            }
            catch (Exception e) { MessageBoxHelper.ShowError("AddDeck", e); }
        }

        public static async Task RenameDeck(MagicDeck deck)
        {
            using var DB = App.Biz.DB.GetNewContext();
            if (deck == null) return;
            string newTitle = MessageBoxHelper.UserInputString("Please enter a title for the deck \"" + deck.Title + "\"", deck.Title);
            if (newTitle == null || string.IsNullOrEmpty(newTitle)) return;
            if (DB.Decks.Where(x => x.Title == newTitle).Any())
            {
                MessageBoxHelper.ShowMsg("There is already a deck with that name.");
                return;
            }
            deck.Title = newTitle;
            DB.Entry(deck).State = EntityState.Modified;
            await DB.SaveChangesAsync();
            App.Events.RaiseUpdateDeck();
        }

        public static async Task DuplicateDeck(MagicDeck deckToCopy)
        {
            if (deckToCopy == null) return;
            var newDeck = new MagicDeck(deckToCopy.Title + " - Copie");
            using (var DB = App.Biz.DB.GetNewContext())
            {
                newDeck.CardRelations = new ObservableCollection<CardDeckRelation>();
                DB.Decks.Add(newDeck);
                DB.Entry(newDeck).State = EntityState.Added;
                foreach (CardDeckRelation relation in deckToCopy.CardRelations)
                {
                    var cardRelation = new CardDeckRelation()
                    {
                        Card = relation.Card,
                        Deck = newDeck,
                        Quantity = relation.Quantity,
                        RelationType = relation.RelationType
                    };
                    newDeck.CardRelations.Add(cardRelation);
                    newDeck.CardCount += relation.Quantity;
                    DB.Entry(cardRelation).State = EntityState.Added;
                }
                await DB.SaveChangesAsync();
            }
            App.Events.RaiseUpdateDeckList();
        }

        public static async Task DeleteDeck(MagicDeck deckToDelete)
        {
            if (MessageBoxHelper.AskUser("Are you sure to delete this deck ? (" + deckToDelete.Title + ")"))
            {
                using var DB = App.Biz.DB.GetNewContext();
                var deck = deckToDelete;
                DB.Decks.Remove(deck);
                await DB.SaveChangesAsync();
                App.Events.RaiseUpdateDeckList();
            }
        }

        public static async Task AddCardToDeck(MagicCardVariant card, MagicDeck deck, int qty, int relation = 0)
        {
            if (card == null || deck == null) return;
            try {
                using (var DB = App.Biz.DB.GetNewContext())
                {
                    var cardRelation = deck.CardRelations.Where(x => x.Card.Card.CardId == card.Card.CardId).FirstOrDefault();
                    if (cardRelation == null)
                    {
                        cardRelation = new CardDeckRelation()
                        {
                            Card = card,
                            CardId = card.Id,
                            Deck = deck,
                            Quantity = qty,
                            RelationType = relation
                        };
                        DB.Entry(cardRelation).State = EntityState.Added;
                        deck.CardRelations.Add(cardRelation);
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
                App.Events.RaiseUpdateDeck();
            }
            catch (Exception ex) { MessageBoxHelper.ShowError("AddCardToDeck", ex); }
        }

        public static async Task RemoveCardFromDeck(MagicCard card, MagicDeck deck, int qty = 1)
        {
            if (card == null || deck == null) return;
            try {
                using (var DB = App.Biz.DB.GetNewContext())
                {
                    var cardRelation = deck.CardRelations.Where(x => x.Card.Card.CardId == card.CardId).FirstOrDefault();
                    if (cardRelation == null) return;
                    cardRelation.Quantity -= qty;
                    if (cardRelation.Quantity <= 0)
                    {
                        deck.CardRelations.Remove(cardRelation);
                        DB.Entry(cardRelation).State = EntityState.Deleted;
                    }
                    else
                    {
                        DB.Entry(cardRelation).State = EntityState.Modified;
                    }
                    deck.CardCount -= qty;
                    DB.Entry(deck).State = EntityState.Modified;
                    await DB.SaveChangesAsync();
                }
                App.Events.RaiseUpdateDeck();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowMsg(ex.Message);
            }
        }

        public static async Task ChangeCardDeckRelation(CardDeckRelation relation, int type)
        {
            using (var DB = App.Biz.DB.GetNewContext())
            {
                relation.RelationType = type;
                DB.Entry(relation).State = EntityState.Modified;
                await DB.SaveChangesAsync();
            }
            App.Events.RaiseUpdateDeck();
        }

        public static async Task ChangeVariant(CardDeckRelation cardDeckRelation, MagicCardVariant magicCardVariant)
        {
            int qty = cardDeckRelation.Quantity;
            var deck = cardDeckRelation.Deck;
            int rel = cardDeckRelation.RelationType;
            await RemoveCardFromDeck(cardDeckRelation.Card.Card, deck, qty);
            await AddCardToDeck(magicCardVariant, deck, qty, rel);
        }

        #endregion

        #region Tags

        public static async Task<List<CardTag>> GetTagsDistinct()
        {
            using var DB = App.Biz.DB.GetNewContext();
            return await DB.Tags.GroupBy(x => x.Tag).Select(x => x.First()).ToListAsync();
        }

        public static async Task<bool> DoesCardHasTag(string cardId, string tagFilterSelected)
        {
            return (await FindTagsForCard(cardId)).Where(x => x.Tag == tagFilterSelected).Any();
        }

        public static async Task TagCard(MagicCard selectedCard, string text)
        {
            using var DB = App.Biz.DB.GetNewContext();
            DB.Tags.Add(new CardTag(text, selectedCard));
            await DB.SaveChangesAsync();
        }

        public static async Task UnTagCard(CardTag cardTag)
        {
            using var DB = App.Biz.DB.GetNewContext();
            DB.Tags.Remove(cardTag);
            await DB.SaveChangesAsync();
        }

        public static async Task<List<CardTag>> FindTagsForCard(string cardId)
        {
            using var DB = App.Biz.DB.GetNewContext();
            return await DB.Tags.Where(x => x.CardId == cardId).ToListAsync();
        }

        #endregion

        #endregion

        #region Statistics

        // TODO : Too Slow, see after card devotions todo
        public static async Task<string> DeckColors(MagicDeck deck)
        {
            string retour = "";
            await Task.Run(() => {
                if (deck.CardRelations.Where(x => x.Card.Card.ManaCost.Contains('B')).Any()) retour += "B";
                if (deck.CardRelations.Where(x => x.Card.Card.ManaCost.Contains('W')).Any()) retour += "W";
                if (deck.CardRelations.Where(x => x.Card.Card.ManaCost.Contains('U')).Any()) retour += "U";
                if (deck.CardRelations.Where(x => x.Card.Card.ManaCost.Contains('G')).Any()) retour += "G";
                if (deck.CardRelations.Where(x => x.Card.Card.ManaCost.Contains('R')).Any()) retour += "R";
            });
            return retour;
        }

        public static async Task<float> EstimateDeckPrice(MagicDeck selectedDeck)
        {
            float total = 0;
            await Task.Run(() => {
                foreach (var v in selectedDeck.CardRelations)
                {
                    float price = float.Parse(v.Card.ValueEur);
                    total += v.Quantity * price;
                }
            });
            return total;
        }

        #region Devotions

        public static async Task<int> DevotionB(MagicDeck deck)
        {
            if (deck == null) return 0;
            if (deck.CardRelations == null) return 0;
            int devotion = 0;
            foreach (var c in deck.CardRelations) devotion += await DevotionB(c.Card.Card) * c.Quantity;
            return devotion;
        }
        public static async Task<int> DevotionW(MagicDeck deck)
        {
            if (deck == null) return 0;
            if (deck.CardRelations == null) return 0;
            int devotion = 0;
            foreach (var c in deck.CardRelations) devotion += await DevotionW(c.Card.Card) * c.Quantity;
            return devotion;
        }
        public static async Task<int> DevotionU(MagicDeck deck)
        {
            if (deck == null) return 0;
            if (deck.CardRelations == null) return 0;
            int devotion = 0;
            foreach (var c in deck.CardRelations) devotion += await DevotionU(c.Card.Card) * c.Quantity;
            return devotion;
        }
        public static async Task<int> DevotionG(MagicDeck deck)
        {
            if (deck == null) return 0;
            if (deck.CardRelations == null) return 0;
            int devotion = 0;
            foreach (var c in deck.CardRelations) devotion += await DevotionG(c.Card.Card) * c.Quantity;
            return devotion;
        }
        public static async Task<int> DevotionR(MagicDeck deck)
        {
            if (deck == null) return 0;
            if (deck.CardRelations == null) return 0;
            int devotion = 0;
            foreach (var c in deck.CardRelations) devotion += await DevotionR(c.Card.Card) * c.Quantity;
            return devotion;
        }

        #endregion

        #region counts

        public static async Task<int> Count_Total(MagicDeck deck)
        {
            int count = 0;
            await Task.Run(() => {
                if (deck != null && deck.CardRelations != null)
                {
                    foreach (var card in deck.CardRelations.Where(x => x.RelationType < 2))
                    {
                        count += card.Quantity;
                    }
                }
            });
            return count;
        }

        public static async Task<int> Count_Creature(MagicDeck deck)
        {
            int count = 0;
            await Task.Run(() => {
                if (deck != null && deck.CardRelations != null)
                {
                    foreach (var v in deck.CardRelations.Where(x => x.Card != null && x.Card.Card.Type.ToLower().Contains("creature")))
                    {
                        count += v.Quantity;
                    };
                }
            });
            return count;
        }

        public static async Task<int> Count_Instant(MagicDeck deck)
        {
            int count = 0;
            await Task.Run(() => {
                if (deck != null && deck.CardRelations != null)
                {
                    foreach (var v in deck.CardRelations.Where(x => x.Card.Card.Type.ToLower().Contains("instant")))
                    {
                        count += v.Quantity;
                    };
                }
            });
            return count;
        }

        public static async Task<int> Count_Sorcery(MagicDeck deck)
        {
            int count = 0;
            await Task.Run(() => {
                if (deck != null && deck.CardRelations != null)
                {
                    foreach (var v in deck.CardRelations.Where(x => x.Card.Card.Type.ToLower().Contains("sorcery")))
                    {
                        count += v.Quantity;
                    };
                }
            });
            return count;
        }

        public static async Task<int> Count_Enchantment(MagicDeck deck)
        {
            int count = 0;
            await Task.Run(() => {
                if (deck != null && deck.CardRelations != null)
                {
                    foreach (var v in deck.CardRelations.Where(x => x.Card.Card.Type.ToLower().Contains("enchantment")))
                    {
                        count += v.Quantity;
                    };
                }
            });
            return count;
        }

        public static async Task<int> Count_Artifact(MagicDeck deck)
        {
            int count = 0;
            await Task.Run(() => {
                if (deck != null && deck.CardRelations != null)
                {
                    foreach (var v in deck.CardRelations.Where(x => x.Card.Card.Type.ToLower().Contains("artifact")))
                    {
                        count += v.Quantity;
                    };
                }
            });
            return count;
        }

        public static async Task<int> Count_BasicLand(MagicDeck deck)
        {
            int count = 0;
            await Task.Run(() => {
                if (deck != null && deck.CardRelations != null)
                {
                    foreach (var v in deck.CardRelations.Where(x => x.Card.Card.Type.ToLower().Contains("basic land")))
                    {
                        count += v.Quantity;
                    };
                }
            });
            return count;
        }

        public static async Task<int> Count_SpecialLand(MagicDeck deck)
        {
            int count = 0;
            await Task.Run(() => {
                if (deck != null && deck.CardRelations != null)
                {
                    foreach (var v in deck.CardRelations.Where(x => x.Card.Card.Type.ToLower().Contains("land") && !x.Card.Card.Type.ToLower().Contains("basic")))
                    {
                        count += v.Quantity;
                    };
                }
            });
            return count;
        }

        public static async Task<int> Count_other(MagicDeck deck)
        {

            int count = 0;
            await Task.Run(() => {
                if (deck != null && deck.CardRelations != null)
                {
                    foreach (var v in deck.CardRelations.Where(x =>
                        !x.Card.Card.Type.ToLower().Contains("creature") &&
                        !x.Card.Card.Type.ToLower().Contains("instant") &&
                        !x.Card.Card.Type.ToLower().Contains("sorcery") &&
                        !x.Card.Card.Type.ToLower().Contains("enchantment") &&
                        !x.Card.Card.Type.ToLower().Contains("artifact") &&
                        !x.Card.Card.Type.ToLower().Contains("land")
                    ))
                    {
                        count += v.Quantity;
                    };
                }
            });
            return count;
        }

        #endregion

        #region Types

        public static async Task<IEnumerable<CardDeckRelation>> GetCommanders(MagicDeck deck)
        {
            IEnumerable<CardDeckRelation> rels = new List<CardDeckRelation>();
            if (deck == null || deck.CardRelations == null) return rels;
            await Task.Run(() => {
                rels = deck.CardRelations.Where(x => x.RelationType == 1);
            });
            return rels;
        }
        public static async Task<IEnumerable<CardDeckRelation>> GetCreatures(MagicDeck deck)
        {
            IEnumerable<CardDeckRelation> rels = new List<CardDeckRelation>();
            if (deck == null || deck.CardRelations == null) return rels;
            await Task.Run(() => {
                rels = deck.CardRelations.Where(
                x => x.RelationType == 0
                && x.Card != null && x.Card.Card.Type.ToLower().Contains("creature"))
                .OrderBy(x => x.Card.Card.Cmc)
                .ThenBy(x => x.Card.Card.CardForeignName);
            });
            return rels;
        }
        public static async Task<IEnumerable<CardDeckRelation>> GetInstants(MagicDeck deck)
        {
            IEnumerable<CardDeckRelation> rels = new List<CardDeckRelation>();
            if (deck == null || deck.CardRelations == null) return rels;
            await Task.Run(() => {
                rels = deck.CardRelations.Where(
                x => x.RelationType == 0
                && x.Card != null && x.Card.Card.Type.ToLower().Contains("instant"))
                .OrderBy(x => x.Card.Card.Cmc)
                .ThenBy(x => x.Card.Card.CardForeignName);
            });
            return rels;
        }
        public static async Task<IEnumerable<CardDeckRelation>> GetSorceries(MagicDeck deck)
        {
            IEnumerable<CardDeckRelation> rels = new List<CardDeckRelation>();
            if (deck == null || deck.CardRelations == null) return rels;
            await Task.Run(() => {
                rels = deck.CardRelations.Where(
                x => x.RelationType == 0
                && x.Card != null && x.Card.Card.Type.ToLower().Contains("sorcery"))
                .OrderBy(x => x.Card.Card.Cmc)
                .ThenBy(x => x.Card.Card.CardForeignName);
            });
            return rels;
        }
        public static async Task<IEnumerable<CardDeckRelation>> GetEnchantments(MagicDeck deck)
        {
            IEnumerable<CardDeckRelation> rels = new List<CardDeckRelation>();
            if (deck == null || deck.CardRelations == null) return rels;
            await Task.Run(() => {
                rels = deck.CardRelations.Where(
                x => x.RelationType == 0
                && x.Card != null && x.Card.Card.Type.ToLower().Contains("enchantment"))
                .OrderBy(x => x.Card.Card.Cmc)
                .ThenBy(x => x.Card.Card.CardForeignName);
            });
            return rels;
        }
        public static async Task<IEnumerable<CardDeckRelation>> GetCurrentArtifacts(MagicDeck deck)
        {
            IEnumerable<CardDeckRelation> rels = new List<CardDeckRelation>();
            if (deck == null || deck.CardRelations == null) return rels;
            await Task.Run(() => {
                rels = deck.CardRelations.Where(
                x => x.RelationType == 0
                && x.Card != null
                && x.Card.Card.Type.ToLower().Contains("artifact"))
                .OrderBy(x => x.Card.Card.Cmc)
                .ThenBy(x => x.Card.Card.CardForeignName);
            });
            return rels;
        }
        public static async Task<IEnumerable<CardDeckRelation>> GetCurrentNonBasicLands(MagicDeck deck)
        {
            IEnumerable<CardDeckRelation> rels = new List<CardDeckRelation>();
            if (deck == null || deck.CardRelations == null) return rels;
            await Task.Run(() => {
                rels = deck.CardRelations.Where(
                x => x.RelationType == 0
                && x.Card != null
                && x.Card.Card.Type.ToLower().Contains("land")
                && !x.Card.Card.Type.ToLower().Contains("basic"))
                .OrderBy(x => x.Card.Card.Cmc)
                .ThenBy(x => x.Card.Card.CardForeignName);
            });
            return rels;
        }
        public static async Task<IEnumerable<CardDeckRelation>> GetCurrentBasicLands(MagicDeck deck)
        {
            IEnumerable<CardDeckRelation> rels = new List<CardDeckRelation>();
            if (deck == null || deck.CardRelations == null) return rels;
            await Task.Run(() => {
                rels = deck.CardRelations.Where(
                x => x.RelationType == 0
                && x.Card != null
                && x.Card.Card.Type.ToLower().Contains("land")
                && x.Card.Card.Type.ToLower().Contains("basic"))
                .OrderBy(x => x.Card.Card.Cmc)
                .ThenBy(x => x.Card.Card.CardForeignName);
            });
            return rels;
        }
        public static async Task<IEnumerable<CardDeckRelation>> GetCurrentOthers(MagicDeck deck)
        {
            IEnumerable<CardDeckRelation> rels = new List<CardDeckRelation>();
            if (deck == null || deck.CardRelations == null) return rels;
            await Task.Run(() => {
                rels = deck.CardRelations.Where(
                x => x.RelationType == 0
                && x.Card != null
                && !x.Card.Card.Type.ToLower().Contains("artifact")
                && !x.Card.Card.Type.ToLower().Contains("creature")
                && !x.Card.Card.Type.ToLower().Contains("instant")
                && !x.Card.Card.Type.ToLower().Contains("sorcery")
                && !x.Card.Card.Type.ToLower().Contains("enchantment")
                && !x.Card.Card.Type.ToLower().Contains("land"))
                .OrderBy(x => x.Card.Card.Cmc)
                .ThenBy(x => x.Card.Card.CardForeignName);
            });
            return rels;
        }
        public static async Task<IEnumerable<CardDeckRelation>> GetCurrentSide(MagicDeck deck)
        {
            IEnumerable<CardDeckRelation> rels = new List<CardDeckRelation>();
            if (deck == null || deck.CardRelations == null) return rels;
            await Task.Run(() => {
                rels = deck.CardRelations.Where(
                x => x.RelationType == 2
                && x.Card != null)
                .OrderBy(x => x.Card.Card.Cmc)
                .ThenBy(x => x.Card.Card.CardForeignName);
            });
            return rels;
        }

        #endregion

        #region Indicators

        public static async Task<int[]> GetManaCurve(MagicDeck deck)
        {
            var manaCurve = new int[11];
            await Task.Run(() => {
                manaCurve[0] = deck.CardRelations.Where(x => !x.Card.Card.Type.ToLower().Contains("land") && x.Card.Card.Cmc == 0).Count();
                manaCurve[1] = deck.CardRelations.Where(x => x.Card.Card.Cmc == 1).Count();
                manaCurve[2] = deck.CardRelations.Where(x => x.Card.Card.Cmc == 2).Count();
                manaCurve[3] = deck.CardRelations.Where(x => x.Card.Card.Cmc == 3).Count();
                manaCurve[4] = deck.CardRelations.Where(x => x.Card.Card.Cmc == 4).Count();
                manaCurve[5] = deck.CardRelations.Where(x => x.Card.Card.Cmc == 5).Count();
                manaCurve[6] = deck.CardRelations.Where(x => x.Card.Card.Cmc == 6).Count();
                manaCurve[7] = deck.CardRelations.Where(x => x.Card.Card.Cmc == 7).Count();
                manaCurve[8] = deck.CardRelations.Where(x => x.Card.Card.Cmc == 8).Count();
                manaCurve[9] = deck.CardRelations.Where(x => x.Card.Card.Cmc == 9).Count();
                manaCurve[10] = deck.CardRelations.Where(x => x.Card.Card.Cmc >= 10).Count();
            });
            return manaCurve;
        }

        public static async Task<int> OwnedRatio(MagicDeck currentDeck)
        {
            if (currentDeck == null) return 0;
            if (currentDeck.CardRelations == null) return 0;
            int total = 0;
            int miss = 0;
            await Task.Run(() => {
                foreach (var v in currentDeck.CardRelations)
                {
                    total += v.Quantity;

                    int got = v.Card.Got;
                    int need = v.Quantity;
                    int diff = need - got;
                    if (diff > 0) miss += diff;
                }
            });
            if (total == 0) return 100;
            return 100 - miss * 100 / total;
        }

        public static async Task<string> ListMissingCards(MagicDeck currentDeck)
        {
            if (currentDeck == null) return null;
            if (currentDeck.CardRelations == null) return null;
            string missList = "";
            await Task.Run(() => {
                foreach (var v in currentDeck.CardRelations)
                {
                    int got = v.Card.Got;
                    int need = v.Quantity;
                    int diff = need - got;
                    if (diff > 0) missList += diff + " " + v.Card.Card.CardId + "\n";
                }
            });
            return missList;
        }

        #endregion

        // TODO : all formats, check card validity
        #region Validities

        public static async Task<bool> Validity_Standard(MagicDeck deck)
        {
            if (deck == null) return false;
            bool ok = true;

            ok = ok && deck.CardCount >= 60;
            ok = ok && await RespectsMaxCardOccurence(deck, 4);

            return ok;
        }

        public static async Task<bool> Validity_Commander(MagicDeck deck)
        {
            if (deck == null) return false;
            bool ok = true;
            ok = ok && deck.CardCount == 100;
            ok = ok && await RespectsMaxCardOccurence(deck, 1);
            ok = ok && deck.CardRelations.Where(x => x.RelationType == 1).Any();
            return ok;
        }

        private static async Task<bool> RespectsMaxCardOccurence(MagicDeck deck, int limit)
        {
            if (deck == null) return false;
            bool ok = true;
            await Task.Run(() => {
                foreach (var v in deck.CardRelations.Where(x => !x.Card.Card.Type.ToString().ToLower().Contains("land")))
                {
                    if (v.Quantity > limit) ok = false;
                }
            });
            return ok;
        }

        #endregion

        #endregion

    }

}
