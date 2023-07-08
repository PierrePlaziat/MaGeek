using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using MaGeek.Entities;
using MaGeek.Framework.Utils;

namespace MaGeek.AppBusiness
{

    public static class MageekCollection
    {

        #region Cards

        public static async Task<CardModel> QuickFindCardById(string cardId)
        {
            using var DB = App.DB.NewContext;
            return await DB.CardModels.Where(x => x.CardId == cardId)
                            .FirstOrDefaultAsync();
        }
        
        public static async Task<CardModel> FindCardById(string cardId)
        {
            using var DB = App.DB.NewContext;
            return await DB.CardModels.Where(x => x.CardId == cardId)
                            .Include(card => card.Traductions)
                            .Include(card => card.Variants)
                            .FirstOrDefaultAsync();
        }

        public static async Task GotCard_Add(CardVariant selectedCard)
        {
            if (selectedCard == null) return;
            using var DB = App.DB.NewContext;
            var variant = DB.CardVariants.Where(x => x.Id == selectedCard.Id)
                .Include(x=>x.Card)
                .FirstOrDefault();

            var gotLine = await DB.User_GotCards.Where(x => x.CardVariantId == selectedCard.Id).FirstOrDefaultAsync();
            if(gotLine == null)
            {
                // Create
                DB.User_GotCards.Add(new User_GotCard() {
                    CardModelId = selectedCard.Card.CardId,
                    CardVariantId = selectedCard.Id,
                    got = 1
                });
            }
            else
            {
                // Update
                gotLine.got++;
                DB.Entry(gotLine).State = EntityState.Modified;
            }
            // Auto Fav
            if (string.IsNullOrEmpty(variant.Card.FavouriteVariant))
            {
                variant.Card.FavouriteVariant = variant.Id;
                //DB.Entry(variant).State = EntityState.Modified;
            }
            await DB.SaveChangesAsync();
        }

        public static async Task GotCard_Remove(CardVariant selectedCard)
        {
            if (selectedCard == null) return;
            using var DB = App.DB.NewContext;
            var c = DB.CardVariants.Where(x => x.Id == selectedCard.Id)
                .Include(x => x.Card)
                .FirstOrDefault();

            var gotLine = await DB.User_GotCards.Where(x => x.CardVariantId == selectedCard.Id).FirstOrDefaultAsync();
            if (gotLine == null) return;
            else
            {
                // Update
                gotLine.got--;
                if (gotLine.got < 0) gotLine.got = 0;
                DB.Entry(gotLine).State = EntityState.Modified;
            }

            DB.Entry(c).State = EntityState.Modified;
            await DB.SaveChangesAsync();
        }

        internal static async Task<int> GotCard_HaveOne(CardModel card)
        {
            using var DB = App.DB.NewContext;
            int retour = 0;

            var v = DB.User_GotCards.Where(x => x.CardModelId == card.CardId);
            foreach(var vv in v)
            {
                retour += vv.got;
            }
            return retour;
        }
        
        internal static async Task<int> GotCard_HaveOne(CardVariant card, bool onlyThisVariant)
        {
            using var DB = App.DB.NewContext;
            if (onlyThisVariant)
            {
                var v = await DB.User_GotCards.Where(x => x.CardVariantId == card.Id).FirstOrDefaultAsync();
                return v != null ? v.got : 0;
            }
            else
            {
                int retour = 0;

                var v = DB.User_GotCards.Where(x => x.CardModelId == card.Id).ToList();
                foreach(var vv in v)
                {
                    retour += vv.got;
                }
                return retour;
            }
        }

        public static async Task SetFav(CardModel card, CardVariant variant)
        {
            using var DB = App.DB.NewContext;
            card.FavouriteVariant = variant.Id;
            DB.Entry(card).State = EntityState.Modified;
            await DB.SaveChangesAsync();
        }

        public static async Task<string> GetEnglishNameFromForeignName(string foreignName, string lang)
        {
            string englishName = "";
            try
            {
                using var DB = App.DB.NewContext;
                {
                    var t = await DB.CardTraductions.Where(x => x.TraductedName == foreignName).FirstOrDefaultAsync();
                    if (t != null)
                    {
                        englishName = t.CardId;
                    }
                }
            }
            catch (Exception e) { Log.Write(e); }
            return englishName;
        }

        public static async Task<string> GetTraduction(string englishName)
        {
            string foreignName = "";
            try
            {
                string lang = App.Config.Settings[Setting.ForeignLanguage];
                using var DB = App.DB.NewContext;
                {
                    var t = await DB.CardTraductions.Where(x => x.CardId == englishName && x.Language == lang).FirstOrDefaultAsync();
                    if (t != null)
                    {
                        foreignName = t.TraductedName;
                    }
                }
            }
            catch (Exception e) { Log.Write(e); }
            return foreignName;
        }

        #endregion

        #region Decks

        public static async Task<List<Deck>> GetDecks()
        {
            List<Deck> decks = new();
            try
            {
                using (var DB = App.DB.NewContext)
                {
                    decks = await DB.Decks
                        .Include(deck => deck.DeckCards)
                            .ThenInclude(cardrel => cardrel.Card)
                                .ThenInclude(card => card.Card)
                        .Include(deck => deck.DeckCards)
                            .ThenInclude(cardrel => cardrel.Card)
                                .ThenInclude(card => card.DeckRelations)
                        .ToListAsync();
                }
            }
            catch (Exception e) { Log.Write(e); }
            return decks;
        }

        public static async Task AddEmptyDeck()
        {
            try
            {
                using var DB = App.DB.NewContext;
                string deckTitle = Log.GetInputFromUser("Please enter a title for this new deck", "");
                if (deckTitle == null) return;
                if (DB.Decks.Where(x => x.Title == deckTitle).Any())
                {
                    Log.Write("There is already a deck with that name.");
                    return;
                }
                Deck deck = new(deckTitle);
                DB.Decks.Add(deck);
                await DB.SaveChangesAsync();
                App.Events.RaiseUpdateDeckList();
                App.Events.RaiseDeckSelect(deck);
            }
            catch (Exception e) { Log.Write(e); }
        }

        public static async Task AddDeck(List<ImportLine> importLines, string title)
        {
            try
            {
                var deck = new Deck(title);
                using (var DB = App.DB.NewContext)
                {
                    DB.Decks.Add(deck);
                    await DB.SaveChangesAsync();
                }
                CardModel card;
                foreach (var cardOccurence in importLines)
                {
                    if (!string.IsNullOrEmpty(cardOccurence.Name))
                    {

                        using (var DB = App.DB.NewContext)
                        {
                            card = DB.CardModels.Where(x => x.CardId == cardOccurence.Name)
                                                .Include(x => x.Variants)
                                                .FirstOrDefault();
                            if (card == null)
                            {
                                card = DB.CardModels.Where(x => x.CardId.StartsWith(cardOccurence.Name + " // "))
                                                    .Include(x => x.Variants)
                                                    .FirstOrDefault();
                            }
                            if (card != null)
                            {
                                CardVariant variant = card.Variants[0];
                                var cardRelation = deck.DeckCards.Where(x => x.Card.Card.CardId == card.CardId).FirstOrDefault();
                                if (cardRelation == null)
                                {
                                    cardRelation = new DeckCard()
                                    {
                                        Card = variant,
                                        CardId = variant.Id,
                                        Deck = deck,
                                        Quantity = cardOccurence.Quantity,
                                        RelationType = cardOccurence.Side ? 2 : 0
                                    };
                                    DB.Entry(cardRelation).State = EntityState.Added;
                                    deck.DeckCards.Add(cardRelation);
                                }
                                else
                                {
                                    cardRelation.Quantity += cardOccurence.Quantity;
                                    DB.Entry(cardRelation).State = EntityState.Modified;
                                }
                                deck.CardCount += cardOccurence.Quantity;
                            }
                            else Log.Write("Couldnt find card : " + cardOccurence.Name);
                            DB.Entry(deck).State = EntityState.Modified;
                            await DB.SaveChangesAsync();
                        }
                    }
                    else { 
                        throw new Exception("??? cardOccurence was null");
                    }
                }
            }
            catch (Exception e) { Log.Write(e); }
        }

        public static async Task RenameDeck(Deck deck)
        {
            using var DB = App.DB.  NewContext;
            if (deck == null) return;
            string newTitle = Log.GetInputFromUser("Please enter a title for the deck \"" + deck.Title + "\"", deck.Title);
            if (newTitle == null || string.IsNullOrEmpty(newTitle)) return;
            if (DB.Decks.Where(x => x.Title == newTitle).Any())
            {
                Log.Write("There is already a deck with that name.");
                return;
            }
            deck.Title = newTitle;
            DB.Entry(deck).State = EntityState.Modified;
            await DB.SaveChangesAsync();
            App.Events.RaiseUpdateDeck();
        }

        public static async Task DuplicateDeck(Deck deckToCopy)
        {
            if (deckToCopy == null) return;
            var newDeck = new Deck(deckToCopy.Title + " - Copie");
            using (var DB = App.DB.NewContext)
            {
                newDeck.DeckCards = new ObservableCollection<DeckCard>();
                DB.Decks.Add(newDeck);
                DB.Entry(newDeck).State = EntityState.Added;
                foreach (DeckCard relation in deckToCopy.DeckCards)
                {
                    var cardRelation = new DeckCard()
                    {
                        Card = relation.Card,
                        Deck = newDeck,
                        Quantity = relation.Quantity,
                        RelationType = relation.RelationType
                    };
                    newDeck.DeckCards.Add(cardRelation);
                    newDeck.CardCount += relation.Quantity;
                    DB.Entry(cardRelation).State = EntityState.Added;
                }
                await DB.SaveChangesAsync();
            }
            App.Events.RaiseUpdateDeckList();
        }

        //TODO use a string builder
        public static async Task<string> GetDeckTxt(Deck deck)
        {
            if (deck == null) return "null";
            string result = "";
            result += deck.Title + "\n\n";
            foreach (var v in deck.DeckCards)
            {
                result += v.Quantity + " " + v.Card.Card.CardId + "\n";
            }
            return result;
        }

        public static async Task DeleteDeck(Deck deckToDelete)
        {
            if (Log.AskUser("Are you sure to delete this deck ? (" + deckToDelete.Title + ")"))
            {
                using var DB = App.DB.NewContext;
                var deck = deckToDelete;
                await DB.SaveChangesAsync();
                App.Events.RaiseUpdateDeckList();
            }
            Log.Write("Done");
        }
        
        public static async Task DeleteDecks(List<Deck> deckToDelete)
        {
            if (Log.AskUser("Are you sure to delete those deck ? (" + deckToDelete.Count + " decks)"))
            {
                using var DB = App.DB.NewContext;
                var deck = deckToDelete;
                DB.Decks.RemoveRange(deck);
                await DB.SaveChangesAsync();
                App.Events.RaiseUpdateDeckList();
            }
            Log.Write("Done");
        }

        public static async Task AddCardToDeck(CardModel card, Deck deck, int qty, int relation = 0)
        {
            using var DB = App.DB.NewContext;
            CardVariant v = await DB.CardVariants.Where(x=>x.Card.CardId == card.CardId).Include(x=>x.Card).FirstOrDefaultAsync();
            if (v != null) await AddCardToDeck(v, deck, qty, relation);
        }

            public static async Task AddCardToDeck(CardVariant card, Deck deck, int qty, int relation = 0)
        {
            if (card == null || deck == null) return;
            try {
                using (var DB = App.DB.NewContext)
                {
                    var cardRelation = deck.DeckCards.Where(x => x.Card.Card.CardId == card.Card.CardId).FirstOrDefault();
                    if (cardRelation == null)
                    {
                        cardRelation = new DeckCard()
                        {
                            Card = card,
                            CardId = card.Id,
                            Deck = deck,
                            Quantity = qty,
                            RelationType = relation
                        };
                        DB.Entry(cardRelation).State = EntityState.Added;
                        deck.DeckCards.Add(cardRelation);
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
            catch (Exception e) { Log.Write(e); }
        }

        public static async Task RemoveCardFromDeck(CardModel card, Deck deck, int qty = 1)
        {
            if (card == null || deck == null) return;
            try {
                using (var DB = App.DB.NewContext)
                {
                    var cardRelation = deck.DeckCards.Where(x => x.Card.Card.CardId == card.CardId).FirstOrDefault();
                    if (cardRelation == null) return;
                    cardRelation.Quantity -= qty;
                    if (cardRelation.Quantity <= 0)
                    {
                        deck.DeckCards.Remove(cardRelation);
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
                Log.Write(ex.Message);
            }
        }

        public static async Task ChangeCardDeckRelation(DeckCard relation, int type)
        {
            using (var DB = App.DB.NewContext)
            {
                relation.RelationType = type;
                DB.Entry(relation).State = EntityState.Modified;
                await DB.SaveChangesAsync();
            }
            App.Events.RaiseUpdateDeck();
        }

        public static async Task ChangeVariant(DeckCard cardDeckRelation, CardVariant magicCardVariant)
        {
            int qty = cardDeckRelation.Quantity;
            var deck = cardDeckRelation.Deck;
            int rel = cardDeckRelation.RelationType;
            await RemoveCardFromDeck(cardDeckRelation.Card.Card, deck, qty);
            await AddCardToDeck(magicCardVariant, deck, qty, rel);
        }

        #endregion

        #region sets

        internal async static Task<List<CardVariant>> GetCardsFromSet(MtgSet set)
        {
            List<CardVariant> variants = new();
            if (set != null)
            {
                try
                {
                    using (var DB = App.DB.NewContext)
                    {
                        variants = await DB.CardVariants.Where(x => x.SetName == set.Name)
                                                  .Include(x => x.Card)
                                                  .ToListAsync();
                    }
                }
                catch (Exception e)
                {
                    Log.Write(e, "SelectSet");
                }
            }
            return variants;
        }

        internal static async Task<int> SetNbOwned(MtgSet set, bool onlyThisVariant)
        {
            int nb = 0;
            try
            {
                var cards = await GetCardsFromSet(set);
                using var DB = App.DB.NewContext;
                {
                    foreach (var c in cards)
                    {
                        if (await GotCard_HaveOne(c, onlyThisVariant) > 0) nb++;
                    }
                }
            }
            catch (Exception e) { Log.Write(e); }
            return nb;
        }

        #endregion

    }

}
