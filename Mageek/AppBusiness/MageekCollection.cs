using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using MaGeek.AppData.Entities;
using Plaziat.CommonWpf;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using MaGeek.AppFramework;

namespace MaGeek.AppBusiness
{

    public static class MageekCollection
    {

        #region Cards

        public static async Task<CardModel> QuickFindCardById(string cardId)
        {
            using var DB = App.DB.GetNewContext();
            return await DB.CardModels.Where(x => x.CardId == cardId)
                            .FirstOrDefaultAsync();
        }
        
        public static async Task<CardModel> FindCardById(string cardId)
        {
            using var DB = App.DB.GetNewContext();
            return await DB.CardModels.Where(x => x.CardId == cardId)
                            .Include(card => card.Traductions)
                            .Include(card => card.Variants)
                            .FirstOrDefaultAsync();
        }

        public static async Task GotCard_Add(CardVariant selectedCard)
        {
            if (selectedCard == null) return;
            using var DB = App.DB.GetNewContext();
            var c = DB.CardVariants.Where(x => x.Id == selectedCard.Id)
                .Include(x=>x.Card)
                .FirstOrDefault();
            c.Got++;
            c.Card.Got++;
            DB.Entry(c).State = EntityState.Modified;
            await DB.SaveChangesAsync();
        }

        public static async Task GotCard_Remove(CardVariant selectedCard)
        {
            if (selectedCard == null) return;
            using var DB = App.DB.GetNewContext();
            var c = DB.CardVariants.Where(x => x.Id == selectedCard.Id)
                .Include(x => x.Card)
                .FirstOrDefault();
            c.Got--;
            c.Card.Got--;
            if (c.Got < 0) c.Got = 0;
            if (c.Card.Got < 0) c.Card.Got = 0;
            DB.Entry(c).State = EntityState.Modified;
            await DB.SaveChangesAsync();
        }

        public static async Task SetFav(CardModel card, CardVariant variant)
        {
            using var DB = App.DB.GetNewContext();
            card.FavouriteVariant = variant.Id;
            DB.Entry(card).State = EntityState.Modified;
            await DB.SaveChangesAsync();
        }

        public static async Task<string> GetEnglishNameFromForeignName(string foreignName, string lang)
        {
            string englishName = "";
            try
            {
                using var DB = App.DB.GetNewContext();
                {
                    var t = await DB.CardTraductions.Where(x => x.TraductedName == foreignName).FirstOrDefaultAsync();
                    if (t != null)
                    {
                        englishName = t.CardId;
                    }
                }
            }
            catch (Exception e) { AppLogger.ShowError(MethodBase.GetCurrentMethod().Name, e); }
            return englishName;
        }

        public static async Task<string> GetTraduction(string englishName)
        {
            string foreignName = "";
            try
            {
                string lang = App.Config.Settings[Setting.ForeignLanguage];
                using var DB = App.DB.GetNewContext();
                {
                    var t = await DB.CardTraductions.Where(x => x.CardId == englishName && x.Language == lang).FirstOrDefaultAsync();
                    if (t != null)
                    {
                        foreignName = t.TraductedName;
                    }
                }
            }
            catch (Exception e) { AppLogger.ShowError(MethodBase.GetCurrentMethod().Name, e); }
            return foreignName;
        }
        
        #endregion

        #region Decks

        public static async Task<List<Deck>> GetDecks()
        {
            List<Deck> decks = new();
            try
            {
                using (var DB = App.DB.GetNewContext())
                {
                    decks = await DB.Decks
                        .Include(deck => deck.CardRelations)
                            .ThenInclude(cardrel => cardrel.Card)
                                .ThenInclude(card => card.Card)
                        .Include(deck => deck.CardRelations)
                            .ThenInclude(cardrel => cardrel.Card)
                                .ThenInclude(card => card.DeckRelations)
                        .ToListAsync();
                }
            }
            catch (Exception e) { AppLogger.ShowError(MethodBase.GetCurrentMethod().Name, e); }
            return decks;
        }

        public static async Task AddEmptyDeck()
        {
            try
            {
                using var DB = App.DB.GetNewContext();
                string deckTitle = AppLogger.UserInputString("Please enter a title for this new deck", "");
                if (deckTitle == null) return;
                if (DB.Decks.Where(x => x.Title == deckTitle).Any())
                {
                    AppLogger.ShowMsg("There is already a deck with that name.");
                    return;
                }
                Deck deck = new(deckTitle);
                DB.Decks.Add(deck);
                await DB.SaveChangesAsync();
                App.Events.RaiseUpdateDeckList();
                App.Events.RaiseDeckSelect(deck);
            }
            catch (Exception e) { AppLogger.ShowError("AddEmptyDeck", e); }
        }

        public static async Task AddDeck(List<ImportLine> importLines, string title)
        {
            try
            {
                var deck = new Deck(title);
                using (var DB = App.DB.GetNewContext())
                {
                    DB.Decks.Add(deck);
                    await DB.SaveChangesAsync();
                }
                CardModel card;
                foreach (var cardOccurence in importLines)
                {
                    if (!string.IsNullOrEmpty(cardOccurence.Name))
                    {

                        using (var DB = App.DB.GetNewContext())
                        {
                            card = DB.CardModels.Where(x => x.CardId == cardOccurence.Name)
                                                .Include(x => x.Variants)
                                                .FirstOrDefault();
                            if (card != null)
                            {
                                CardVariant variant = card.Variants[0];
                                var cardRelation = deck.CardRelations.Where(x => x.Card.Card.CardId == card.CardId).FirstOrDefault();
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
                                    deck.CardRelations.Add(cardRelation);
                                }
                                else
                                {
                                    cardRelation.Quantity += cardOccurence.Quantity;
                                    DB.Entry(cardRelation).State = EntityState.Modified;
                                }
                                deck.CardCount += cardOccurence.Quantity;
                            }
                        DB.Entry(deck).State = EntityState.Modified;
                        await DB.SaveChangesAsync();
                        }
                    }
                    else { 
                        throw new Exception("??? cardOccurence was null");
                    }
                }
            }
            catch (Exception e) { AppLogger.ShowError(MethodBase.GetCurrentMethod().Name, e); }
        }

        public static async Task RenameDeck(Deck deck)
        {
            using var DB = App.DB.GetNewContext();
            if (deck == null) return;
            string newTitle = AppLogger.UserInputString("Please enter a title for the deck \"" + deck.Title + "\"", deck.Title);
            if (newTitle == null || string.IsNullOrEmpty(newTitle)) return;
            if (DB.Decks.Where(x => x.Title == newTitle).Any())
            {
                AppLogger.ShowMsg("There is already a deck with that name.");
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
            using (var DB = App.DB.GetNewContext())
            {
                newDeck.CardRelations = new ObservableCollection<DeckCard>();
                DB.Decks.Add(newDeck);
                DB.Entry(newDeck).State = EntityState.Added;
                foreach (DeckCard relation in deckToCopy.CardRelations)
                {
                    var cardRelation = new DeckCard()
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

        public static async Task DeleteDeck(Deck deckToDelete)
        {
            if (AppLogger.AskUser("Are you sure to delete this deck ? (" + deckToDelete.Title + ")"))
            {
                using var DB = App.DB.GetNewContext();
                var deck = deckToDelete;
                await DB.SaveChangesAsync();
                App.Events.RaiseUpdateDeckList();
            }
        }
        
        public static async Task DeleteDecks(List<Deck> deckToDelete)
        {
            if (AppLogger.AskUser("Are you sure to delete those deck ? (" + deckToDelete.Count + " decks)"))
            {
                using var DB = App.DB.GetNewContext();
                var deck = deckToDelete;
                DB.Decks.RemoveRange(deck);
                await DB.SaveChangesAsync();
                App.Events.RaiseUpdateDeckList();
            }
        }

        public static async Task AddCardToDeck(CardModel card, Deck deck, int qty, int relation = 0)
        {
            using var DB = App.DB.GetNewContext();
            CardVariant v = await DB.CardVariants.Where(x=>x.Card.CardId == card.CardId).Include(x=>x.Card).FirstOrDefaultAsync();
            if (v != null) await AddCardToDeck(v, deck, qty, relation);
        }

            public static async Task AddCardToDeck(CardVariant card, Deck deck, int qty, int relation = 0)
        {
            if (card == null || deck == null) return;
            try {
                using (var DB = App.DB.GetNewContext())
                {
                    var cardRelation = deck.CardRelations.Where(x => x.Card.Card.CardId == card.Card.CardId).FirstOrDefault();
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
            catch (Exception ex) { AppLogger.ShowError("AddCardToDeck", ex); }
        }

        public static async Task RemoveCardFromDeck(CardModel card, Deck deck, int qty = 1)
        {
            if (card == null || deck == null) return;
            try {
                using (var DB = App.DB.GetNewContext())
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
                AppLogger.ShowMsg(ex.Message);
            }
        }

        public static async Task ChangeCardDeckRelation(DeckCard relation, int type)
        {
            using (var DB = App.DB.GetNewContext())
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

    }

}
