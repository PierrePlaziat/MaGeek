using MaGeek.Data;
using MaGeek.Data.Entities;
using MtgApiManager.Lib.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;

namespace MaGeek
{


    public partial class App : Application
    {

        public static AppDbContext appContext = new AppDbContext();

        public static MagicDeck CurrentDeck { get; set; }

        internal static void SaveCard(MagicCard selectedCard)
        {
            if(!appContext.cards.Where(x => x.Name_VO == selectedCard.Name_VO).Any())
            {
                appContext.cards.Add(selectedCard);
                foreach(var cardVariant in selectedCard.variants)
                {
                    appContext.cardVariants.Add(cardVariant);
                }
                appContext.UpdateCollection();
            }
        }

        public static void Collection_AddCard(MagicCard selectedCard)
        {
            appContext.cards.Where(x => x.Name_VO == selectedCard.Name_VO).FirstOrDefault().CollectedQuantity++;
            appContext.UpdateCollection();
        }

        internal static void AddCardToDeck(MagicCard card, MagicDeck deck)
        {
            if (deck.Cards == null) App.CurrentDeck.Cards = new List<MagicCard>();
            deck.Cards.Add(card);
            appContext.SaveChanges();
        }

        public static void Collection_RemoveCard(MagicCard selectedCard)
        {
            appContext.cards.Where(x => x.Name_VO == selectedCard.Name_VO).FirstOrDefault().CollectedQuantity--;
            appContext.UpdateCollection();
        }

    }

}
