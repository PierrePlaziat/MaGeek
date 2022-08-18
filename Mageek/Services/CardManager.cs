using MaGeek.Data.Entities;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;

namespace MaGeek.Data
{

    public class CardManager
    {

        public IMtgApi Api { get; } = new MtgApi();
        public ObservableCollection<MagicCard> CardListBinder
        {
            get
            {
                App.Database.cards.Load();
                return App.Database.cards.Local.ToObservableCollection();
            }
        }
        public ObservableCollection<MagicDeck> DeckListBinder
        {
            get
            {
                App.Database.decks.Load();
                return App.Database.decks.Local.ToObservableCollection();
            }
        }

        #region Card Manips

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
            App.State.ModifDeck();
        }

        internal void ChangeRelation(CardDeckRelation cardDeckRelation, MagicCardVariant magicCardVariant)
        {
            int qty = cardDeckRelation.Quantity;
            var deck = cardDeckRelation.Deck;
            int rel = cardDeckRelation.RelationType;
            RemoveCardFromDeck(cardDeckRelation.Card.Card, cardDeckRelation.Deck, cardDeckRelation.Quantity);
            AddCardToDeck(magicCardVariant, deck, qty,rel);
        }

        public void RemoveCardFromDeck(MagicCard card, MagicDeck deck, int qty = 1)
        {
            var cardRelation = deck.CardRelations.Where(x => x.Card.Card.CardId == card.CardId).FirstOrDefault();
            if (cardRelation == null) return;
            cardRelation.Quantity -= qty;
            if (cardRelation.Quantity <= 0) deck.CardRelations.Remove(cardRelation);
            App.Database.SaveChanges();
            App.State.ModifDeck();

        }

        #endregion

    }

}
