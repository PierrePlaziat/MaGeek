﻿using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace MaGeek.AppData.Entities
{
    public class MagicDeck
    {

        #region Entity

        [Key]
        public int DeckId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string DeckColors { get; set; } //  { return App.Biz.Utils.DeckColors(this); } } 
        public int CardCount { get; set; } 

        public virtual ObservableCollection<CardDeckRelation> CardRelations { get; set; }

        #endregion

        #region CTOR

        public MagicDeck() { } // EF needs

        public MagicDeck(string deckTitle)
        {
            Title = deckTitle;
            CardRelations = new ObservableCollection<CardDeckRelation>();
        }

        public MagicDeck(MagicDeck deckToCopy)
        {
            Title = deckToCopy.Title + " - Copie";
            CardRelations = new ObservableCollection<CardDeckRelation>();
            foreach (CardDeckRelation relation in deckToCopy.CardRelations)
            {
                App.Biz.Utils.AddCardToDeck(relation.Card, this, relation.Quantity, relation.RelationType);
            }
        }

        #endregion

    }
}
