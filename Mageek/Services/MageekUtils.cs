using MaGeek.Data.Entities;
using MaGeek.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MaGeek.Data
{

    public class MageekUtils
    {

        #region Deck Manips

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
            App.Database.SaveChanges();
            App.State.RaiseUpdateDeck();
        }

        internal void ChangeRelation(CardDeckRelation cardDeckRelation, MagicCardVariant magicCardVariant)
        {
            int qty = cardDeckRelation.Quantity;
            var deck = cardDeckRelation.Deck;
            int rel = cardDeckRelation.RelationType;
            RemoveCardFromDeck(cardDeckRelation.Card.Card, cardDeckRelation.Deck, cardDeckRelation.Quantity);
            AddCardToDeck(magicCardVariant, deck, qty, rel);
        }

        public void RemoveCardFromDeck(MagicCard card, MagicDeck deck, int qty = 1)
        {
            var cardRelation = deck.CardRelations.Where(x => x.Card.Card.CardId == card.CardId).FirstOrDefault();
            if (cardRelation == null) return;
            cardRelation.Quantity -= qty;
            if (cardRelation.Quantity <= 0) deck.CardRelations.Remove(cardRelation);
            App.Database.SaveChanges();
            App.State.RaiseUpdateDeck();

        }

        public void GotCard_Add(MagicCard selectedCard)
        {
            if (selectedCard == null) return;
            App.Database.cards.Where(x => x.CardId == selectedCard.CardId).FirstOrDefault().CollectedQuantity++;
            App.Database.SaveChanges();
        }

        public void GotCard_Remove(MagicCard selectedCard)
        {
            if (selectedCard == null) return;
            var c = App.Database.cards.Where(x => x.CardId == selectedCard.CardId).FirstOrDefault();
            c.CollectedQuantity--;
            if(c.CollectedQuantity < 0) c.CollectedQuantity = 0;
            App.Database.SaveChanges();
        }

        public void SetFav(MagicCard card, string variantId)
        {
            card.FavouriteVariant = variantId;
            App.Database.SaveChanges();
        }

        #endregion

        #region counts

        public int count_Total(MagicDeck deck)
        { 
            int count = 0;
            if (deck.CardRelations != null)
            {
                foreach (var card in deck.CardRelations.Where(x => x.RelationType<2))
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

        #region Validities

        public bool validity_Standard(MagicDeck deck)
        {
            if (deck == null) return false;
            bool ok = true;
            ok = ok && count_Total (deck)>= 60;
            ok = ok && HasMaxCardOccurence(deck,4);
            return ok;
        }

        public bool validity_Commander(MagicDeck deck)
        {
            if (deck == null) return false;
            bool ok = true;
            ok = ok && count_Total(deck) == 100;
            ok = ok && HasMaxCardOccurence(deck,1);
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

        #endregion

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
        

        #region Colors

        public string DeckColors (MagicDeck deck) 
        { 
            string retour = "";
            if (DevotionB(deck) > 0) retour += "b";
            if (DevotionW(deck) > 0) retour += "w";
            if (DevotionU(deck) > 0) retour += "u";
            if (DevotionG(deck) > 0) retour += "g";
            if (DevotionR(deck) > 0) retour += "r";
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
            if (deck== null) return 0;
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


        private int DevotionB (MagicCard card)
        { 
            return card.ManaCost != null ? 
                card.ManaCost.Length - card.ManaCost.Replace("B", "").Length 
                : 0; 
        }

        private int DevotionW (MagicCard card)
        { 
            return card.ManaCost != null ? 
                card.ManaCost.Length - card.ManaCost.Replace("W", "").Length 
                : 0; 
        }

        private int DevotionU (MagicCard card)
        { 
            return card.ManaCost != null ? 
                card.ManaCost.Length - card.ManaCost.Replace("U", "").Length 
                : 0; 
        }

        private int DevotionG (MagicCard card)
        { 
            return card.ManaCost != null ? 
                card.ManaCost.Length - card.ManaCost.Replace("G", "").Length 
                : 0; 
        }

        private int DevotionR (MagicCard card)
        { 
            return card.ManaCost != null ? 
                card.ManaCost.Length - card.ManaCost.Replace("R", "").Length 
                : 0; 
        }

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

    }

}