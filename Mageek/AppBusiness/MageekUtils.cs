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

    /// <summary>
    /// All business logic
    /// Full static async functionnal style
    /// </summary>
    public static class MageekUtils
    {

        #region Collection

        #region Cards

        public static async Task<MagicCard> FindCardById(string cardId)
        {
            using var DB = App.DB.GetNewContext();
            return await DB.Cards.Where(x => x.CardId == cardId)
                            .Include(card => card.Traductions)
                            .Include(card => card.Variants)
                            .FirstOrDefaultAsync();
        }

        public static async Task GotCard_Add(MagicCardVariant selectedCard)
        {
            if (selectedCard == null) return;
            using var DB = App.DB.GetNewContext();
            var c = DB.CardVariants.Where(x => x.Id == selectedCard.Id).FirstOrDefault();
            c.Got++;
            DB.Entry(c).State = EntityState.Modified;
            await DB.SaveChangesAsync();
        }

        public static async Task GotCard_Remove(MagicCardVariant selectedCard)
        {
            if (selectedCard == null) return;
            using var DB = App.DB.GetNewContext();
            var c = DB.CardVariants.Where(x => x.Id == selectedCard.Id).FirstOrDefault();
            c.Got--;
            if (c.Got < 0) c.Got = 0;
            DB.Entry(c).State = EntityState.Modified;
            await DB.SaveChangesAsync();
        }

        public static async Task SetFav(MagicCard card, MagicCardVariant variant)
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
            catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }
            return englishName;
        }

        public static async Task<string> GetTraduction(string englishName)
        {
            string foreignName = "";
            try
            {
                string lang = App.Config.Settings[Setting.ForeignLangugage];
                using var DB = App.DB.GetNewContext();
                {
                    var t = await DB.CardTraductions.Where(x => x.CardId == englishName && x.Language == lang).FirstOrDefaultAsync();
                    if (t != null)
                    {
                        foreignName = t.TraductedName;
                    }
                }
            }
            catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }
            return foreignName;
        }

        #endregion

        #region Decks

        public static async Task<List<MagicDeck>> GetDecks()
        {
            List<MagicDeck> decks = new();
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
            catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }
            return decks;
        }

        public static async Task AddEmptyDeck()
        {
            try
            {
                using var DB = App.DB.GetNewContext();
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
                using var DB = App.DB.GetNewContext();
                {
                    var deck = new MagicDeck(title);
                    DB.Decks.Add(deck);
                    await DB.SaveChangesAsync();
                    MagicCard card;
                    foreach (var cardOccurence in importLines)
                    {
                        if (!string.IsNullOrEmpty(cardOccurence.Name))
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
                        else
                        {
                            //???
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
            using var DB = App.DB.GetNewContext();
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
            using (var DB = App.DB.GetNewContext())
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
                using var DB = App.DB.GetNewContext();
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
                using (var DB = App.DB.GetNewContext())
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
                MessageBoxHelper.ShowMsg(ex.Message);
            }
        }

        public static async Task ChangeCardDeckRelation(CardDeckRelation relation, int type)
        {
            using (var DB = App.DB.GetNewContext())
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
            using var DB = App.DB.GetNewContext();
            return await DB.Tags.GroupBy(x => x.Tag).Select(x => x.First()).ToListAsync();
        }

        public static async Task<bool> DoesCardHasTag(string cardId, string tagFilterSelected)
        {
            return (await FindTagsForCard(cardId)).Where(x => x.Tag == tagFilterSelected).Any();
        }

        public static async Task TagCard(MagicCard selectedCard, string text)
        {
            using var DB = App.DB.GetNewContext();
            DB.Tags.Add(new CardTag(text, selectedCard));
            await DB.SaveChangesAsync();
        }

        public static async Task UnTagCard(CardTag cardTag)
        {
            using var DB = App.DB.GetNewContext();
            DB.Tags.Remove(cardTag);
            await DB.SaveChangesAsync();
        }

        public static async Task<List<CardTag>> FindTagsForCard(string cardId)
        {
            using var DB = App.DB.GetNewContext();
            return await DB.Tags.Where(x => x.CardId == cardId).ToListAsync();
        }

        #endregion

        #endregion

        #region Statistics

        // TODO : Too Slow
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
            await Task.Run(() =>
            {
                foreach (var c in deck.CardRelations) devotion += c.Card.Card.DevotionB;
            });
            return devotion;
        }
        public static async Task<int> DevotionW(MagicDeck deck)
        {
            if (deck == null) return 0;
            if (deck.CardRelations == null) return 0;
            int devotion = 0;
            await Task.Run(() =>
            {
                foreach (var c in deck.CardRelations) devotion += c.Card.Card.DevotionW;
            });
            return devotion;
        }
        public static async Task<int> DevotionU(MagicDeck deck)
        {
            if (deck == null) return 0;
            if (deck.CardRelations == null) return 0;
            int devotion = 0;
            await Task.Run(() =>
            {
                foreach (var c in deck.CardRelations) devotion += c.Card.Card.DevotionU;
            });
            return devotion;
        }
        public static async Task<int> DevotionG(MagicDeck deck)
        {
            if (deck == null) return 0;
            if (deck.CardRelations == null) return 0;
            int devotion = 0;
            await Task.Run(() =>
            {
                foreach (var c in deck.CardRelations) devotion += c.Card.Card.DevotionG;
            });
            return devotion;
        }
        public static async Task<int> DevotionR(MagicDeck deck)
        {
            if (deck == null) return 0;
            if (deck.CardRelations == null) return 0;
            int devotion = 0;
            await Task.Run(() =>
            {
                foreach (var c in deck.CardRelations) devotion += c.Card.Card.DevotionR;
            });
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

        #region Validities

        public static async Task<string> Validity_Standard(MagicDeck deck)
        {
            if (deck == null) return "";
            if (deck.CardCount < 60) return "Min 60 cards needed";
            if (!await RespectsMaxCardOccurence(deck, 4)) return "No more than 4 times the same card needed";
            using (var DB = App.DB.GetNewContext())
            {
                foreach (var v in deck.CardRelations)
                {
                    if (!v.Card.Card.Type.Contains("Basic Land"))
                    if (DB.Legalities.Where(x=> x.CardId==v.Card.Id && x.Format== "legacy" && x.IsLegal == "legal").Any())
                    {
                        return v.Card.Card.CardId+" is not legal";
                    }
                }
            }
            return "OK";
        }

        public static async Task<string> Validity_Commander(MagicDeck deck)
        {
            if (deck == null) return "";
            if (deck.CardCount != 100) return "Exctly 100 cards needed";
            if (!await RespectsMaxCardOccurence(deck, 1)) return "No more than 1 times the same card needed.";
            using (var DB = App.DB.GetNewContext())
            {
                foreach (var v in deck.CardRelations)
                {
                    if (!v.Card.Card.Type.Contains("Basic Land"))
                        if (DB.Legalities.Where(x => x.CardId == v.Card.Id && x.Format == "commander" && x.IsLegal == "legal").Any())
                        {
                            return v.Card.Card.CardId + " is not legal";
                        }
                }
            }
            return "OK";
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
