using MaGeek.AppData.Entities;
using Plaziat.CommonWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MaGeek.AppData;
using ScryfallApi.Client.Models;

namespace MaGeek.AppBusiness
{

    public class MageekUtils
    {

        #region CTOR

        public MageekDbContext db;
        public MageekDbContext DB
        {
            get
            {
                if (db == null) db = App.Biz.DB.GetNewContext();
                return db;
            }
        }

        public ScryfallManager ScryfallManager;

        public MageekUtils()
        {
            ScryfallManager = new ScryfallManager();
        }

        #endregion

        #region Deck Manips

        internal void ChangeCardDeckRelation(CardDeckRelation cardRel, int v)
        {
            cardRel.RelationType = 0;
            DB.SaveChanges();
            App.Events.RaiseUpdateDeck();
        }

        public void AddDeck()
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
                DB.SaveChanges();
                App.Events.RaiseUpdateDeckList();
                App.Events.RaiseDeckSelect(deck);
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowMsg(ex.Message);
            }
        }

        public void RenameDeck(MagicDeck deckToRename)
        {
            if (deckToRename == null) return;
            string newTitle = MessageBoxHelper.UserInputString("Please enter a title for the deck \"" + deckToRename.Title + "\"", deckToRename.Title);
            if (newTitle == null || string.IsNullOrEmpty(newTitle)) return;
            if (DB.decks.Where(x => x.Title == newTitle).Any())
            {
                MessageBoxHelper.ShowMsg("There is already a deck with that name.");
                return;
            }
            deckToRename.Title = newTitle;
            DB.SaveChanges();
            App.Events.RaiseUpdateDeck();
        }

        public void DuplicateDeck(MagicDeck originalDeck)
        {
            if (App.State.SelectedDeck == null) return;
            var deckToCopy = originalDeck;
            var newDeck = new MagicDeck(deckToCopy);
            DB.decks.Add(newDeck);
            DB.SaveChanges();
            App.Events.RaiseUpdateDeckList();
        }

        public void DeleteDeck(MagicDeck deckToDelete)
        {
            //if (decklistbox.SelectedIndex >= 0 && decklistbox.SelectedIndex < Decks.Count)
            if (MessageBoxHelper.AskUser("Are you sure to delete this deck ? (" + deckToDelete.Title + ")"))
            {
                var deck = deckToDelete;
                DB.decks.Remove(deck);
                DB.SaveChanges();
                App.Events.RaiseUpdateDeckList();
            }
        }

        public void ChangeRelation(CardDeckRelation cardDeckRelation, MagicCardVariant magicCardVariant)
        {
            int qty = cardDeckRelation.Quantity;
            var deck = cardDeckRelation.Deck;
            int rel = cardDeckRelation.RelationType;
            RemoveCardFromDeck(cardDeckRelation.Card.Card, cardDeckRelation.Deck, cardDeckRelation.Quantity);
            AddCardToDeck(magicCardVariant, deck, qty, rel);
        }

        public void AddCardToDeck(MagicCardVariant card, MagicDeck deck, int qty, int relation = 0)
        {
            if (card == null || deck == null) return;
            var cardRelation = deck.CardRelations.Where(x => x.Card.Card.CardId == card.Card.CardId).FirstOrDefault();
            if (cardRelation == null)
            {
                cardRelation = new CardDeckRelation()
                {
                    Card = card,
                    Deck = deck,
                    Quantity = 0,
                    RelationType = relation
                };
                deck.CardRelations.Add(cardRelation);
            }
            cardRelation.Quantity += qty;
            deck.CardCount += qty; 
            DB.SaveChanges();
            App.Events.RaiseUpdateDeck();
        }

        public void RemoveCardFromDeck(MagicCard card, MagicDeck deck, int qty = 1)
        {
            var cardRelation = deck.CardRelations.Where(x => x.Card.Card.CardId == card.CardId).FirstOrDefault();
            if (cardRelation == null) return;
            cardRelation.Quantity -= qty;
            if (cardRelation.Quantity <= 0) deck.CardRelations.Remove(cardRelation);
            deck.CardCount -= qty;
            DB.SaveChanges();
            App.Events.RaiseUpdateDeck();

        }

        #endregion

        #region Card Manips

        internal MagicCard FindCardById(string cardId)
        {
            return DB.cards.Where(x => x.CardId == cardId).FirstOrDefault();
        }

        public void GotCard_Add(MagicCardVariant selectedCard)
        {
            if (selectedCard == null) return;
            DB.cardVariants.Where(x => x.Id == selectedCard.Id).FirstOrDefault().Got++;
            DB.SaveChanges();
        }

        public void GotCard_Remove(MagicCardVariant selectedCard)
        {
            if (selectedCard == null) return;
            var c = DB.cardVariants.Where(x => x.Id == selectedCard.Id).FirstOrDefault();
            c.Got--;
            if (c.Got < 0) c.Got = 0;
            DB.SaveChanges();
        }

        public void SetFav(MagicCard card, string variantId)
        {
            card.FavouriteVariant = variantId;
            DB.SaveChanges();
        }

        #endregion

        #region Tags

        public List<CardTag> GetTagsDistinct()
        {
            return DB.Tags.GroupBy(x => x.Tag).Select(x => x.First()).ToList();
        }

        internal bool DoesCardHasTag(string cardId, string tagFilterSelected)
        {
            return FindTagsForCard(cardId).Where(x => x.Tag == tagFilterSelected).Any();
        }

        internal void TagCard(MagicCard selectedCard, string text)
        {
            DB.Tags.Add(new CardTag(text, selectedCard));
            DB.SaveChanges();
        }

        internal void UnTagCard(CardTag cardTag)
        {
            DB.Tags.Remove(cardTag);
            DB.SaveChanges();
        }

        internal List<CardTag> FindTagsForCard(string cardId)
        {
            return DB.Tags.Where(x => x.CardId == cardId).ToList();
        }

        #endregion

        #region Mana

        public int[] GetManaCurve(MagicDeck deck)
        {
            var manaCurve = new int[11];
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
            return manaCurve;
        }

        //SLOW
        public string DeckColors(MagicDeck deck)
        {
            string retour = "";
            if (deck.CardRelations.Where(x => x.Card.Card.ManaCost.Contains("B")).Any()) retour += "B";
            if (deck.CardRelations.Where(x => x.Card.Card.ManaCost.Contains("W")).Any()) retour += "W";
            if (deck.CardRelations.Where(x => x.Card.Card.ManaCost.Contains("U")).Any()) retour += "U";
            if (deck.CardRelations.Where(x => x.Card.Card.ManaCost.Contains("G")).Any()) retour += "G";
            if (deck.CardRelations.Where(x => x.Card.Card.ManaCost.Contains("R")).Any()) retour += "R";
            return retour;
        }

        public int DevotionB(MagicDeck deck)
        {
            if (deck == null) return 0;
            if (deck.CardRelations == null) return 0;
            int devotion = 0;
            foreach (var c in deck.CardRelations) devotion += DevotionB(c.Card.Card) * c.Quantity;
            return devotion;
        }
        public int DevotionW(MagicDeck deck)
        {
            if (deck == null) return 0;
            if (deck.CardRelations == null) return 0;
            int devotion = 0;
            foreach (var c in deck.CardRelations) devotion += DevotionW(c.Card.Card) * c.Quantity;
            return devotion;
        }
        public int DevotionU(MagicDeck deck)
        {
            if (deck == null) return 0;
            if (deck.CardRelations == null) return 0;
            int devotion = 0;
            foreach (var c in deck.CardRelations) devotion += DevotionU(c.Card.Card) * c.Quantity;
            return devotion;
        }
        public int DevotionG(MagicDeck deck)
        {
            if (deck == null) return 0;
            if (deck.CardRelations == null) return 0;
            int devotion = 0;
            foreach (var c in deck.CardRelations) devotion += DevotionG(c.Card.Card) * c.Quantity;
            return devotion;
        }
        public int DevotionR(MagicDeck deck)
        {
            if (deck == null) return 0;
            if (deck.CardRelations == null) return 0;
            int devotion = 0;
            foreach (var c in deck.CardRelations) devotion += DevotionR(c.Card.Card) * c.Quantity;
            return devotion;
        }

        private int DevotionB(MagicCard card)
        {
            return card.ManaCost != null ?
                card.ManaCost.Length - card.ManaCost.Replace("B", "").Length
                : 0;
        }
        private int DevotionW(MagicCard card)
        {
            return card.ManaCost != null ?
                card.ManaCost.Length - card.ManaCost.Replace("W", "").Length
                : 0;
        }
        private int DevotionU(MagicCard card)
        {
            return card.ManaCost != null ?
                card.ManaCost.Length - card.ManaCost.Replace("U", "").Length
                : 0;
        }
        private int DevotionG(MagicCard card)
        {
            return card.ManaCost != null ?
                card.ManaCost.Length - card.ManaCost.Replace("G", "").Length
                : 0;
        }
        private int DevotionR(MagicCard card)
        {
            return card.ManaCost != null ?
                card.ManaCost.Length - card.ManaCost.Replace("R", "").Length
                : 0;
        }

        #endregion

        //SLOW
        #region counts

        public int count_Total(MagicDeck deck)
        {
            int count = 0;
            if (deck.CardRelations != null)
            {
                foreach (var card in deck.CardRelations.Where(x => x.RelationType < 2))
                {
                    count += card.Quantity;
                }
            }
            return count;
        }

        public int count_Creature(MagicDeck deck)
        {
            int count = 0;
            if (deck != null)
            {
                foreach (var v in deck.CardRelations.Where(x => x.Card.Card.Type.ToLower().Contains("creature")))
                {
                    count += v.Quantity;
                };
            }
            return count;
        }

        public int count_Instant(MagicDeck deck)
        {
            int count = 0;
            if (deck != null)
            {
                foreach (var v in deck.CardRelations.Where(x => x.Card.Card.Type.ToLower().Contains("instant")))
                {
                    count += v.Quantity;
                };
            }
            return count;
        }

        public int count_Sorcery(MagicDeck deck)
        {
            int count = 0;
            if (deck != null)
            {
                foreach (var v in deck.CardRelations.Where(x => x.Card.Card.Type.ToLower().Contains("sorcery")))
                {
                    count += v.Quantity;
                };
            }
            return count;
        }

        public int count_Enchantment(MagicDeck deck)
        {
            int count = 0;
            if (deck != null)
            {
                foreach (var v in deck.CardRelations.Where(x => x.Card.Card.Type.ToLower().Contains("enchantment")))
                {
                    count += v.Quantity;
                };
            }
            return count;
        }

        public int count_Artifact(MagicDeck deck)
        {
            int count = 0;
            if (deck != null)
            {
                foreach (var v in deck.CardRelations.Where(x => x.Card.Card.Type.ToLower().Contains("artifact")))
                {
                    count += v.Quantity;
                };
            }
            return count;
        }

        public int count_BasicLand(MagicDeck deck)
        {
            int count = 0;
            if (deck != null)
            {
                foreach (var v in deck.CardRelations.Where(x => x.Card.Card.Type.ToLower().Contains("basic land")))
                {
                    count += v.Quantity;
                };
            }
            return count;
        }

        public int count_SpecialLand(MagicDeck deck)
        {
            int count = 0;
            if (deck != null)
            {
                foreach (var v in deck.CardRelations.Where(x => x.Card.Card.Type.ToLower().Contains("land") && !x.Card.Card.Type.ToLower().Contains("basic")))
                {
                    count += v.Quantity;
                };
            }
            return count;
        }

        //TODO
        public int count_other(MagicDeck deck)
        {

            int count = 0;
            if (deck != null)
            {
                foreach (var v in deck.CardRelations.Where(x => x.Card.Card.Type.ToLower().Contains("enchantment")))
                {
                    count += v.Quantity;
                };
            }
            return count;
        }

        #endregion

        #region Types

        internal IEnumerable<CardDeckRelation> GetCommanders(MagicDeck deck)
        {
            if (deck == null || deck.CardRelations == null) return null;
            return deck.CardRelations.Where(x => x.RelationType == 1);
        }

        internal IEnumerable<CardDeckRelation> GetCreatures(MagicDeck currentDeck)
        {
            if (currentDeck == null || currentDeck.CardRelations == null) return null;
            return new ObservableCollection<CardDeckRelation>(currentDeck.CardRelations.Where(
                x => x.RelationType == 0
                && x.Card.Card.Type.ToLower().Contains("creature"))
                .OrderBy(x => x.Card.Card.Cmc.Value)
                .ThenBy(x => x.Card.Card.CardForeignName)
            );
        }

        internal IEnumerable<CardDeckRelation> GetInstants(MagicDeck currentDeck)
        {
            if (currentDeck == null || currentDeck.CardRelations == null) return null;
            return new ObservableCollection<CardDeckRelation>(currentDeck.CardRelations.Where(
                x => x.RelationType == 0
                && x.Card.Card.Type.ToLower().Contains("instant"))
                .OrderBy(x => x.Card.Card.Cmc.Value)
                .ThenBy(x => x.Card.Card.CardForeignName)
            );
        }

        internal IEnumerable<CardDeckRelation> GetSorceries(MagicDeck currentDeck)
        {
            if (currentDeck == null || currentDeck.CardRelations == null) return null;
            return new ObservableCollection<CardDeckRelation>(currentDeck.CardRelations.Where(
                x => x.RelationType == 0
                && x.Card.Card.Type.ToLower().Contains("sorcery"))
                .OrderBy(x => x.Card.Card.Cmc.Value)
                .ThenBy(x => x.Card.Card.CardForeignName)
            );
        }

        internal IEnumerable<CardDeckRelation> GetEnchantments(MagicDeck currentDeck)
        {
            if (currentDeck == null || currentDeck.CardRelations == null) return null;
            return new ObservableCollection<CardDeckRelation>(currentDeck.CardRelations.Where(
                x => x.RelationType == 0
                && x.Card.Card.Type.ToLower().Contains("enchantment"))
                .OrderBy(x => x.Card.Card.Cmc.Value)
                .ThenBy(x => x.Card.Card.CardForeignName)
            );
        }

        #endregion

        #region Owned / Missing

        internal int OwnedRatio(MagicDeck currentDeck)
        {
            if (currentDeck == null) return 0;
            if (currentDeck.CardRelations == null) return 0;
            int total = 0;
            int miss = 0;
            foreach (var v in currentDeck.CardRelations)
            {
                total += v.Quantity;

                int got = v.Card.Got;
                int need = v.Quantity;
                int diff = need - got;
                if (diff > 0) miss += diff;
            }
            if (total == 0) return 100;
            return 100 - miss * 100 / total;
        }

        internal string ListMissingCards(MagicDeck currentDeck)
        {
            if (currentDeck == null) return null;
            if (currentDeck.CardRelations == null) return null;
            string missList = "";
            foreach (var v in currentDeck.CardRelations)
            {
                int got = v.Card.Got;
                int need = v.Quantity;
                int diff = need - got;
                if (diff > 0) missList += diff + " " + v.Card.Card.CardId + "\n";
            }
            return missList;
        }

        #endregion

        #region Validities

        public bool validity_Standard(MagicDeck deck)
        {
            if (deck == null) return false;
            bool ok = true;
            ok = ok && count_Total(deck) >= 60;
            ok = ok && HasMaxCardOccurence(deck, 4);
            return ok;
        }

        public bool validity_Commander(MagicDeck deck)
        {
            if (deck == null) return false;
            bool ok = true;
            ok = ok && count_Total(deck) == 100;
            ok = ok && HasMaxCardOccurence(deck, 1);
            ok = ok && deck.CardRelations.Where(x => x.RelationType == 1).Any();
            return ok;
        }

        private bool HasMaxCardOccurence(MagicDeck deck, int limit)
        {
            if (deck == null) return false;
            bool ok = true;
            foreach (var v in deck.CardRelations.Where(x => !x.Card.Card.Type.ToString().ToLower().Contains("land")))
            {
                if (v.Quantity > limit) ok = false;
            }
            return ok;
        }

        internal List<Legality> GetCardLegal(MagicCardVariant selectedVariant)
        {
            return ScryfallManager.GetCardLegal(selectedVariant);
        }

        #endregion

        #region Prices

        internal float EstimateDeckPrice(MagicDeck selectedDeck)
        {
            float total = 0;
            foreach (var v in selectedDeck.CardRelations)
            {
                total += v.Quantity * ScryfallManager.GetCardPrize(v.CardId);
            }
            return total;
        }

        internal float GetCardPrize(MagicCardVariant selectedVariant)
        {
            return ScryfallManager.GetCardPrize(selectedVariant);
        }

        #endregion

    }

}