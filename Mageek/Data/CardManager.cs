﻿using MaGeek.Data.Entities;
using MaGeek.Entities;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MaGeek.Data
{

    public class CardManager
    {

        public ApiMtg MtgApi = new ApiMtg();

        public void AddCardToDeck(MagicCard card, MagicDeck deck, int qty = 1)
        {
            if (card == null || deck == null) return;
            var cardRelation = deck.CardRelations.Where(x => x.CardId == card.CardId).FirstOrDefault();
            if (cardRelation == null)
            {
                cardRelation = new CardDeckRelation() { 
                    Card = card, 
                    Deck = deck, 
                    Quantity = 0 
                };
                deck.CardRelations.Add(cardRelation);
            }
            cardRelation.Quantity += qty;
            App.database.SaveChanges();
            App.state.ModifDeck();
        }
        public void RemoveCardFromDeck(MagicCard card, MagicDeck deck)
        {
            var cardRelation = deck.CardRelations.Where(x => x.CardId == card.CardId).FirstOrDefault();
            if (cardRelation == null) return;
            cardRelation.Quantity--;
            if (cardRelation.Quantity == 0) deck.CardRelations.Remove(cardRelation);
            App.database.SaveChanges();
            App.state.ModifDeck();
        }

        public void GotCard_Add(MagicCard selectedCard)
        {
            if (selectedCard == null) return;
            App.database.cards.Where(x => x.CardId == selectedCard.CardId).FirstOrDefault().CollectedQuantity++;
            App.database.SaveChanges();
        }

        public void GotCard_Remove(MagicCard selectedCard)
        {
            if (selectedCard == null) return;
            var c = App.database.cards.Where(x => x.CardId == selectedCard.CardId).FirstOrDefault();
            c.CollectedQuantity--;
            if(c.CollectedQuantity < 0) c.CollectedQuantity = 0;
            App.database.SaveChanges();
        }

        public ObservableCollection<MagicCard> BinderCards
        {
            get
            {
                App.database.cards.Load();
                return App.database.cards.Local.ToObservableCollection();
            }
        }

        public ObservableCollection<MagicDeck> BinderDeck
        {
            get
            {
                App.database.decks.Load();
                return App.database.decks.Local.ToObservableCollection();
            }
        }

    }

}
