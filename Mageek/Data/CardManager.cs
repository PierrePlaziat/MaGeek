using MaGeek.Data.Entities;
using MtgApiManager.Lib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaGeek.Data
{
    public class CardManager
    {

        MTG_API mtgApi = new MTG_API();

        public async Task SearchCardsOnline(string cardname, bool onlyOne = false)
        {
            var foundCards = await mtgApi.SearchCards(cardname);
            foreach (var foundCard in foundCards)
            {
                if( !onlyOne || (foundCard != null && foundCard.Name == cardname) )
                {
                    MagicCard card = SaveLocalCard(foundCard); 
                    card.AddVariant(foundCard);
                }
            }
            App.database.UpdateCollection();
        }

        private MagicCard SaveLocalCard(ICard iCard)
        {
            // Guard
            var card = FindLocalCard(iCard);
            if (card != null)  return card;
            // Proceed
            card = new MagicCard(iCard);
            App.database.cards.Add(card); //App.Current.Dispatcher.Invoke(delegate { App.database.cards.Add(card); });
            return card;
        }

        private MagicCard FindLocalCard(ICard iCard)
        {
            return App.database.cards.Where(x => x.Name_VO == iCard.Name).FirstOrDefault();
        }

        ///////////////////////////////////////////////////////////////
        
        public void AddCardToDeck(MagicCard card, MagicDeck deck)
        {
            if (deck.Cards == null) App.state.CurrentDeck.Cards = new List<MagicCard>();
            deck.Cards.Add(card);
            App.database.SaveChanges();
        }

        public void GotCard_Add(MagicCard selectedCard)
        {
            App.database.cards.Where(x => x.Name_VO == selectedCard.Name_VO).FirstOrDefault().CollectedQuantity++;
            App.database.UpdateCollection();
        }

        public void GotCard_Remove(MagicCard selectedCard)
        {
            App.database.cards.Where(x => x.Name_VO == selectedCard.Name_VO).FirstOrDefault().CollectedQuantity--;
            App.database.UpdateCollection();
        }

    }

}
