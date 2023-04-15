using MaGeek.AppData.Entities;
using Plaziat.CommonWpf;
using System;
using System.Collections.Generic;
using System.Linq;
using MaGeek.AppData;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using ScryfallApi.Client.Models;
using System.Threading;
using CardValue = MaGeek.AppData.Entities.CardValue;
using System.Windows.Controls;

namespace MaGeek.AppBusiness
{

    public class MageekUtils
    {

        #region Deck Manips

        public async Task ChangeCardDeckRelation(CardDeckRelation relation, int type)
        {
            using (var DB = App.Biz.DB.GetNewContext())
            {
                relation.RelationType = type;
                DB.Entry(relation).State = EntityState.Modified;
                await DB.SaveChangesAsync();
            }
            App.Events.RaiseUpdateDeck();
        }

        public async Task AddDeck()
        {
            using (var DB = App.Biz.DB.GetNewContext())
            {
                try
                {
                    string deckTitle = MessageBoxHelper.UserInputString("Please enter a title for this new deck", "");
                    if (deckTitle == null) return;
                    if (DB.decks.Where(x => x.Title == deckTitle).Any())
                    {
                        MessageBoxHelper.ShowMsg("There is already a deck with that name.");
                        return;
                    }
                    MagicDeck deck = new MagicDeck(deckTitle);
                    DB.decks.Add(deck);
                    await DB.SaveChangesAsync();
                    App.Events.RaiseUpdateDeckList();
                    App.Events.RaiseDeckSelect(deck);
                }
                catch (Exception ex)
                {
                    MessageBoxHelper.ShowMsg(ex.Message);
                }
            }
        }

        public async Task RenameDeck(MagicDeck deck)
        {
            using (var DB = App.Biz.DB.GetNewContext())
            {
                if (deck == null) return;
                string newTitle = MessageBoxHelper.UserInputString("Please enter a title for the deck \"" + deck.Title + "\"", deck.Title);
                if (newTitle == null || string.IsNullOrEmpty(newTitle)) return;
                if (DB.decks.Where(x => x.Title == newTitle).Any())
                {
                    MessageBoxHelper.ShowMsg("There is already a deck with that name.");
                    return;
                }
                deck.Title = newTitle;
                DB.Entry(deck).State = EntityState.Modified;
                await DB.SaveChangesAsync();
                App.Events.RaiseUpdateDeck();
            }
        }

        public async Task DuplicateDeck(MagicDeck deckToCopy)
        {
            if (deckToCopy == null) return;
            var newDeck = new MagicDeck(deckToCopy.Title + " - Copie");
            using (var DB = App.Biz.DB.GetNewContext())
            {
                newDeck.CardRelations = new ObservableCollection<CardDeckRelation>();
                DB.decks.Add(newDeck);
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

        public async Task DeleteDeck(MagicDeck deckToDelete)
        {
            if (MessageBoxHelper.AskUser("Are you sure to delete this deck ? (" + deckToDelete.Title + ")"))
            {
                using (var DB = App.Biz.DB.GetNewContext())
                {
                    var deck = deckToDelete;
                    DB.decks.Remove(deck);
                    await DB.SaveChangesAsync();
                    App.Events.RaiseUpdateDeckList();
                }
            }
        }

        public async Task ChangeVariant(CardDeckRelation cardDeckRelation, MagicCardVariant magicCardVariant)
        {
            int qty = cardDeckRelation.Quantity;
            var deck = cardDeckRelation.Deck;
            int rel = cardDeckRelation.RelationType;
            await RemoveCardFromDeck(cardDeckRelation.Card.Card, deck, qty);
            await AddCardToDeck(magicCardVariant, deck, qty, rel);
        }

        public async Task AddCardToDeck(MagicCardVariant card, MagicDeck deck, int qty, int relation = 0)
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
            catch (Exception ex)
            {
                MessageBoxHelper.ShowMsg(ex.Message);
            }
        }

        public async Task RemoveCardFromDeck(MagicCard card, MagicDeck deck, int qty = 1)
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

        #endregion

        #region Card Manips

        public async Task<MagicCard> FindCardById(string cardId)
        {
            using (var DB = App.Biz.DB.GetNewContext())
            {
                return await DB.cards.Where(x => x.CardId == cardId).FirstOrDefaultAsync();
            }
        }

        public async Task GotCard_Add(MagicCardVariant selectedCard)
        {
            using (var DB = App.Biz.DB.GetNewContext())
            {
                if (selectedCard == null) return;
                
                var c = DB.cardVariants.Where(x => x.Id == selectedCard.Id).FirstOrDefault();
                c.Got++;
                DB.Entry(c).State = EntityState.Modified;
                await DB.SaveChangesAsync();
            }
        }

        public async Task GotCard_Remove(MagicCardVariant selectedCard)
        {
            using (var DB = App.Biz.DB.GetNewContext())
            {
                if (selectedCard == null) return;
                var c = DB.cardVariants.Where(x => x.Id == selectedCard.Id).FirstOrDefault();
                c.Got--;
                if (c.Got < 0) c.Got = 0;
                DB.Entry(c).State = EntityState.Modified;
                await DB.SaveChangesAsync();
            }
        }

        public async Task SetFav(MagicCard card, string variantId)
        {
            using (var DB = App.Biz.DB.GetNewContext())
            {
                card.FavouriteVariant = variantId;
                DB.Entry(card).State = EntityState.Modified;
                await DB.SaveChangesAsync();
            }
        }

        #endregion

        #region Tags

        public async Task<List<CardTag>> GetTagsDistinct()
        {
            using (var DB = App.Biz.DB.GetNewContext())
            {
                return await DB.Tags.GroupBy(x => x.Tag).Select(x => x.First()).ToListAsync();
            }
        }

        public async Task<bool> DoesCardHasTag(string cardId, string tagFilterSelected)
        {
            return (await FindTagsForCard(cardId)).Where(x => x.Tag == tagFilterSelected).Any();
        }

        public async Task TagCard(MagicCard selectedCard, string text)
        {
            using (var DB = App.Biz.DB.GetNewContext())
            {
                DB.Tags.Add(new CardTag(text, selectedCard));
                await DB.SaveChangesAsync();
            }
        }

        public async Task UnTagCard(CardTag cardTag)
        {
            using (var DB = App.Biz.DB.GetNewContext())
            {
                DB.Tags.Remove(cardTag);
                await DB.SaveChangesAsync();
            }
        }

        public async Task<List<CardTag>> FindTagsForCard(string cardId)
        {
            using (var DB = App.Biz.DB.GetNewContext())
            {
                return await DB.Tags.Where(x => x.CardId == cardId).ToListAsync();
            }
        }

        #endregion

        #region Devotions

        public async Task<int> DevotionB(MagicDeck deck)
        {
            if (deck == null) return 0;
            if (deck.CardRelations == null) return 0;
            int devotion = 0;
            foreach (var c in deck.CardRelations) devotion += await DevotionB(c.Card.Card) * c.Quantity;
            return devotion;
        }
        public async Task<int> DevotionW(MagicDeck deck)
        {
            if (deck == null) return 0;
            if (deck.CardRelations == null) return 0;
            int devotion = 0;
            foreach (var c in deck.CardRelations) devotion += await DevotionW(c.Card.Card) * c.Quantity;
            return devotion;
        }
        public async Task<int> DevotionU(MagicDeck deck)
        {
            if (deck == null) return 0;
            if (deck.CardRelations == null) return 0;
            int devotion = 0;
            foreach (var c in deck.CardRelations) devotion += await DevotionU(c.Card.Card) * c.Quantity;
            return devotion;
        }
        public async Task<int> DevotionG(MagicDeck deck)
        {
            if (deck == null) return 0;
            if (deck.CardRelations == null) return 0;
            int devotion = 0;
            foreach (var c in deck.CardRelations) devotion += await DevotionG(c.Card.Card) * c.Quantity;
            return devotion;
        }
        public async Task<int> DevotionR(MagicDeck deck)
        {
            if (deck == null) return 0;
            if (deck.CardRelations == null) return 0;
            int devotion = 0;
            foreach (var c in deck.CardRelations) devotion += await DevotionR(c.Card.Card) * c.Quantity;
            return devotion;
        }

        private async Task<int> DevotionB(MagicCard card)
        {
            int devotion = 0;
            await Task.Run(() => {
                devotion = card.ManaCost != null ?
                card.ManaCost.Length - card.ManaCost.Replace("B", "").Length
                : 0;
            });
            return devotion;
        }
        private async Task<int> DevotionW(MagicCard card)
        {
            int devotion = 0;
            await Task.Run(() => {
                devotion = card.ManaCost != null ?
                card.ManaCost.Length - card.ManaCost.Replace("W", "").Length
                : 0;
            });
            return devotion;
        }
        private async Task<int> DevotionU(MagicCard card)
        {
            int devotion = 0;
            await Task.Run(() => {
                devotion = card.ManaCost != null ?
                card.ManaCost.Length - card.ManaCost.Replace("U", "").Length
                : 0;
            });
            return devotion;
        }
        private async Task<int> DevotionG(MagicCard card)
        {
            int devotion = 0;
            await Task.Run(() => {
                devotion = card.ManaCost != null ?
                card.ManaCost.Length - card.ManaCost.Replace("G", "").Length
                : 0;
            });
            return devotion;
        }
        private async Task<int> DevotionR(MagicCard card)
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

        #region counts

        public async Task<int> count_Total(MagicDeck deck)
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

        public async Task<int> count_Creature(MagicDeck deck)
        {
            int count = 0;
            await Task.Run(() => {
                if (deck != null && deck.CardRelations!=null)
                {
                    foreach (var v in deck.CardRelations.Where(x => x.Card!=null && x.Card.Card.Type.ToLower().Contains("creature")))
                    {
                        count += v.Quantity;
                    };
                }
            });
            return count;
        }

        public async Task<int> count_Instant(MagicDeck deck)
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

        public async Task<int> count_Sorcery(MagicDeck deck)
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

        public async Task<int> count_Enchantment(MagicDeck deck)
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

        public async Task<int> count_Artifact(MagicDeck deck)
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

        public async Task<int> count_BasicLand(MagicDeck deck)
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

        public async Task<int> count_SpecialLand(MagicDeck deck)
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

        public async Task<int> count_other(MagicDeck deck)
        {

            int count = 0;
            await Task.Run(() => {
                if (deck != null && deck.CardRelations != null)
                {
                    foreach (var v in deck.CardRelations.Where(x => 
                        ! x.Card.Card.Type.ToLower().Contains("creature") &&
                        ! x.Card.Card.Type.ToLower().Contains("instant") &&
                        ! x.Card.Card.Type.ToLower().Contains("sorcery") &&
                        ! x.Card.Card.Type.ToLower().Contains("enchantment") &&
                        ! x.Card.Card.Type.ToLower().Contains("artifact") &&
                        ! x.Card.Card.Type.ToLower().Contains("land")
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

        public async Task<IEnumerable<CardDeckRelation>> GetCommanders(MagicDeck deck)
        {
            IEnumerable<CardDeckRelation> rels = new List<CardDeckRelation>();
            if (deck == null || deck.CardRelations == null) return rels;
            await Task.Run(() => {
                rels = deck.CardRelations.Where(x => x.RelationType == 1);
            });
            return rels;
        }
        public async Task<IEnumerable<CardDeckRelation>> GetCreatures(MagicDeck deck)
        {
            IEnumerable<CardDeckRelation> rels = new List<CardDeckRelation>();
            if (deck == null || deck.CardRelations == null) return rels;
            await Task.Run(() => {
                rels = deck.CardRelations.Where(
                x => x.RelationType == 0
                && x.Card != null && x.Card.Card.Type.ToLower().Contains("creature"))
                .OrderBy(x => x.Card.Card.Cmc.Value)
                .ThenBy(x => x.Card.Card.CardForeignName);
            });
            return rels;
        }
        public async Task<IEnumerable<CardDeckRelation>> GetInstants(MagicDeck deck)
        {
            IEnumerable<CardDeckRelation> rels = new List<CardDeckRelation>();
            if (deck == null || deck.CardRelations == null) return rels;
            await Task.Run(() => {
                rels = deck.CardRelations.Where(
                x => x.RelationType == 0
                && x.Card != null && x.Card.Card.Type.ToLower().Contains("instant"))
                .OrderBy(x => x.Card.Card.Cmc.Value)
                .ThenBy(x => x.Card.Card.CardForeignName);
            });
            return rels;
        }
        public async Task<IEnumerable<CardDeckRelation>> GetSorceries(MagicDeck deck)
        {
            IEnumerable<CardDeckRelation> rels = new List<CardDeckRelation>();
            if (deck == null || deck.CardRelations == null) return rels;
            await Task.Run(() => {
                rels = deck.CardRelations.Where(
                x => x.RelationType == 0
                && x.Card != null && x.Card.Card.Type.ToLower().Contains("sorcery"))
                .OrderBy(x => x.Card.Card.Cmc.Value)
                .ThenBy(x => x.Card.Card.CardForeignName);
            });
            return rels;
        }
        public async Task<IEnumerable<CardDeckRelation>> GetEnchantments(MagicDeck deck)
        {
            IEnumerable<CardDeckRelation> rels = new List<CardDeckRelation>();
            if (deck == null || deck.CardRelations == null) return rels;
            await Task.Run(() => {
                rels = deck.CardRelations.Where(
                x => x.RelationType == 0
                && x.Card != null && x.Card.Card.Type.ToLower().Contains("enchantment"))
                .OrderBy(x => x.Card.Card.Cmc.Value)
                .ThenBy(x => x.Card.Card.CardForeignName);
            });
            return rels;
        }
        public async Task<IEnumerable<CardDeckRelation>> GetCurrentArtifacts(MagicDeck deck)
        {
            IEnumerable<CardDeckRelation> rels = new List<CardDeckRelation>();
            if (deck == null || deck.CardRelations == null) return rels;
            await Task.Run(() => {
                rels = deck.CardRelations.Where(
                x => x.RelationType == 0
                && x.Card != null
                && x.Card.Card.Type.ToLower().Contains("artifact"))
                .OrderBy(x => x.Card.Card.Cmc.Value)
                .ThenBy(x => x.Card.Card.CardForeignName);
            });
            return rels;
        }
        public async Task<IEnumerable<CardDeckRelation>> GetCurrentNonBasicLands(MagicDeck deck)
        {
            IEnumerable<CardDeckRelation> rels = new List<CardDeckRelation>();
            if (deck == null || deck.CardRelations == null) return rels;
            await Task.Run(() => {
                rels = deck.CardRelations.Where(
                x => x.RelationType == 0
                && x.Card != null
                && x.Card.Card.Type.ToLower().Contains("land")
                && !x.Card.Card.Type.ToLower().Contains("basic"))
                .OrderBy(x => x.Card.Card.Cmc.Value)
                .ThenBy(x => x.Card.Card.CardForeignName);
            });
            return rels;
        }
        public async Task<IEnumerable<CardDeckRelation>> GetCurrentBasicLands(MagicDeck deck)
        {
            IEnumerable<CardDeckRelation> rels = new List<CardDeckRelation>();
            if (deck == null || deck.CardRelations == null) return rels;
            await Task.Run(() => {
                rels = deck.CardRelations.Where(
                x => x.RelationType == 0
                && x.Card != null
                && x.Card.Card.Type.ToLower().Contains("land")
                && x.Card.Card.Type.ToLower().Contains("basic"))
                .OrderBy(x => x.Card.Card.Cmc.Value)
                .ThenBy(x => x.Card.Card.CardForeignName);
            });
            return rels;
        }
        public async Task<IEnumerable<CardDeckRelation>> GetCurrentOthers(MagicDeck deck)
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
                .OrderBy(x => x.Card.Card.Cmc.Value)
                .ThenBy(x => x.Card.Card.CardForeignName);
            });
            return rels;
        }
        public async Task<IEnumerable<CardDeckRelation>> GetCurrentSide(MagicDeck deck)
        {
            IEnumerable<CardDeckRelation> rels = new List<CardDeckRelation>();
            if (deck == null || deck.CardRelations == null) return rels;
            await Task.Run(() => {
                rels = deck.CardRelations.Where(
                x => x.RelationType == 2
                && x.Card != null)
                .OrderBy(x => x.Card.Card.Cmc.Value)
                .ThenBy(x => x.Card.Card.CardForeignName);
            });
            return rels;
        }

        #endregion

        #region Stats

        public async Task<int[]> GetManaCurve(MagicDeck deck)
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

        public async Task<int> OwnedRatio(MagicDeck currentDeck)
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

        public async Task<string> ListMissingCards(MagicDeck currentDeck)
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

        public async Task<bool> validity_Standard(MagicDeck deck)
        {
            if (deck == null) return false;
            bool ok = true;

            ok = ok && deck.CardCount >= 60;
            ok = ok && await RespectsMaxCardOccurence(deck, 4);

            return ok;
        }

        public async Task<bool> validity_Commander(MagicDeck deck)
        {
            if (deck == null) return false;
            bool ok = true;
            ok = ok && deck.CardCount == 100;
            ok = ok && await RespectsMaxCardOccurence(deck, 1);
            ok = ok && deck.CardRelations.Where(x => x.RelationType == 1).Any();
            return ok;
        }

        private async Task<bool> RespectsMaxCardOccurence(MagicDeck deck, int limit)
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

        #region TimeVariingInfos

        public static async Task<List<Legality>> GetCardLegal(MagicCardVariant variant)
        {
            if (variant == null) return new List<Legality>();
            List<Legality> legal = new();
            using (var DB = App.Biz.DB.GetNewContext())
            {
                List<Legality> previous = DB.Legalities.Where(x => x.MultiverseId == variant.MultiverseId).ToList();

                // NO DATA
                if (previous == null || previous.FirstOrDefault() == null)
                {
                    await RetrieveTimeVariingInfos(variant);
                    legal = DB.Legalities.Where(x => x.MultiverseId == variant.MultiverseId).ToList();
                }
                // OUTDATED
                else if (IsOutDated(previous.FirstOrDefault().LastUpdate, 30))
                {
                    DB.Legalities.RemoveRange(previous);
                    await RetrieveTimeVariingInfos(variant);
                    legal = DB.Legalities.Where(x => x.MultiverseId == variant.MultiverseId).ToList();
                }
                // DATA OK 
                else legal = previous;
            }
            return legal;
        }

        public static async Task<float> EstimateDeckPrice(MagicDeck selectedDeck)
        {
            float total = 0;
            foreach (var v in selectedDeck.CardRelations)
            {
                total += v.Quantity * (await GetPrice(v.Card)).ValueEur;
            }
            return total;
        }

        public static async Task<CardValue> GetPrice(MagicCardVariant variant)
        {
            if (variant == null) return null;
            CardValue price;
            using (var DB = App.Biz.DB.GetNewContext())
            {
                price = DB.Prices.Where(x => x.MultiverseId == variant.MultiverseId).FirstOrDefault();
                // NO DATA
                if (price == null)
                {
                    await RetrieveTimeVariingInfos(variant); 
                    price = DB.Prices.Where(x => x.MultiverseId == variant.MultiverseId).FirstOrDefault();
                }
                // OUTDATED
                else if (IsOutDated(price.LastUpdate, 1))
                {
                    DB.Prices.Remove(price);
                    await RetrieveTimeVariingInfos(variant); 
                    price = DB.Prices.Where(x => x.MultiverseId == variant.MultiverseId).FirstOrDefault();
                }
                // DATA OK 
            }
            return price;
        }

        private static async Task RetrieveTimeVariingInfos(MagicCardVariant variant)
        {
            // Guard
            if (string.IsNullOrEmpty(variant.MultiverseId)) return;
            Thread.Sleep(150);
            // Ask
            var json_data = await HttpUtils.Get("https://api.scryfall.com/cards/multiverse/" + variant.MultiverseId);
            if (json_data == string.Empty) return;
            // Parse
            var scryCard = JsonConvert.DeserializeObject<Card>(json_data);
            float priceEur = scryCard.Prices.Eur.HasValue ? float.Parse(scryCard.Prices.Eur.Value.ToString()) : -1;
            float priceUsd = scryCard.Prices.Usd.HasValue ? float.Parse(scryCard.Prices.Eur.Value.ToString()) : -1;
            // Done
            await SavePrice(variant.MultiverseId, priceEur, priceUsd, scryCard.EdhrecRank);
            await SaveLegality(variant.MultiverseId, scryCard.Legalities);
            await SaveRelated(variant.MultiverseId, scryCard.RelatedUris);
        }

        private static async Task SavePrice(string multiverseId, float priceEur, float priceUsd, int edhRank)
        {
            using var DB = App.Biz.DB.GetNewContext();
            DB.Prices.Add(
                new CardValue()
                {
                    MultiverseId = multiverseId,
                    LastUpdate = DateTime.Now.ToString(),
                    ValueEur = priceEur,
                    ValueUsd= priceUsd,
                    EdhScore = edhRank,
                }
            );
            await DB.SaveChangesAsync();
        }

        private static async Task SaveRelated(string multiverseId, Dictionary<string, Uri> related) //TODO
        {
            throw new NotImplementedException();
        }

        private static async Task SaveLegality(string multiverseId, Dictionary<string, string> legalityDico)
        {
            List<Legality> legal = new();
            foreach (var l in legalityDico)
            {
                legal.Add(new Legality()
                {
                    Format = l.Key,
                    IsLegal = l.Value,
                    LastUpdate = DateTime.Now.ToString(),
                    MultiverseId = multiverseId,
                });
            }
            using var DB = App.Biz.DB.GetNewContext();
            {
                DB.Legalities.AddRange(legal);
                await DB.SaveChangesAsync();
            }
        }

        private static bool IsOutDated(string lastUpdate, int dayLimit)
        {
            DateTime lastUp = DateTime.Parse(lastUpdate);
            if (lastUp < DateTime.Now.AddDays(-dayLimit)) return true;
            else return false;
        }

        #endregion

        // TODO : Too Slow, maybe directly store in deck like card quantity
        public async Task<string> DeckColors(MagicDeck deck)
        {
            string retour = "";
            await Task.Run(() => {
                if (deck.CardRelations.Where(x => x.Card.Card.ManaCost.Contains("B")).Any()) retour += "B";
                if (deck.CardRelations.Where(x => x.Card.Card.ManaCost.Contains("W")).Any()) retour += "W";
                if (deck.CardRelations.Where(x => x.Card.Card.ManaCost.Contains("U")).Any()) retour += "U";
                if (deck.CardRelations.Where(x => x.Card.Card.ManaCost.Contains("G")).Any()) retour += "G";
                if (deck.CardRelations.Where(x => x.Card.Card.ManaCost.Contains("R")).Any()) retour += "R";
            });
            return retour;
        }

        // TODO : WebClient is obsolete
        public static async Task<BitmapImage> RetrieveImage(MagicCardVariant magicCardVariant)
        {
            var taskCompletion = new TaskCompletionSource<BitmapImage>();
            BitmapImage img = null;
            string localFileName = "";
            if (magicCardVariant.IsCustom == 0)
            {
                localFileName = Path.Combine(App.Config.Path_ImageFolder, magicCardVariant.Id + ".png");
                if (!File.Exists(localFileName))
                {
                    await Task.Run(async () =>
                    {
                        await new WebClient().DownloadFileTaskAsync(magicCardVariant.ImageUrl, localFileName);
                    });
                }
            }
            //else
            //{
            //    localFileName = @"./CardsIllus/Custom/" + ImageUrl;
            //}
            var path = Path.GetFullPath(localFileName);
            Uri imgUri = new Uri("file://" + path, UriKind.Absolute);
            img = new BitmapImage(imgUri);
            taskCompletion.SetResult(img);
            return img;
        }

    }

}