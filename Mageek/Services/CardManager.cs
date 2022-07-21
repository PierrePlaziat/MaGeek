using MaGeek.Data.Entities;
using Plaziat.CommonWpf;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Linq;

namespace MaGeek.Data
{

    public class CardManager
    {

        public ApiMtg MtgApi { get; } = new();

        #region DB Management

        public void EraseDb()
        {
            if (!MessageBoxHelper.AskUser("Are you sure?")) return;

            var cardRows = from o in App.database.cards select o;
            foreach (var row in cardRows) App.database.cards.Remove(row);

            var cardVariantsrows = from o in App.database.cardVariants select o;
            foreach (var row in cardVariantsrows) App.database.cardVariants.Remove(row);

            var traductionsrows = from o in App.database.traductions select o;
            foreach (var row in traductionsrows) App.database.traductions.Remove(row);

            var decksrows = from o in App.database.decks select o;
            foreach (var row in decksrows) App.database.decks.Remove(row);

            var cardsInDecksrows = from o in App.database.cardsInDecks select o;
            foreach (var row in cardsInDecksrows) App.database.cardsInDecks.Remove(row);

            App.database.SaveChanges();

            MessageBoxHelper.ShowMsg("DB successfully erased");

            System.Diagnostics.Process.Start(App.ResourceAssembly.Location);
            App.Current.Shutdown();

        }

        public void SaveDb()
        {
            string sourceFile = System.AppDomain.CurrentDomain.BaseDirectory+"Mtg.db";
            Console.WriteLine(sourceFile);
            try
            {
                File.Copy(sourceFile, Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Mtg.db", true);
                MessageBoxHelper.ShowMsg("DB successfully saved");
            }
            catch (IOException iox)
            {
                MessageBoxHelper.ShowMsg(iox.Message);
            }
        }

        public void LoadDb()
        {
            string sourceFile = System.AppDomain.CurrentDomain.BaseDirectory + "Mtg.db";
            Console.WriteLine(sourceFile);
            try
            {
                File.Copy(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Mtg.db", sourceFile,true);
                MessageBoxHelper.ShowMsg("DB successfully loaded");
            }
            catch (IOException iox)
            {
                MessageBoxHelper.ShowMsg(iox.Message);
            }

            System.Diagnostics.Process.Start(App.ResourceAssembly.Location);
            App.Current.Shutdown();
        }

        public void CleanDb()
        {
            //TODO
            throw new NotImplementedException();
        }

        #endregion

        #region Card Manips

        public ObservableCollection<MagicCard> BinderCards
        {
            get
            {
                App.database.cards.Load();
                return App.database.cards.Local.ToObservableCollection();
            }
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

        public void SetFav(MagicCard card, string variantId)
        {
            card.FavouriteVariant = variantId;
            App.database.SaveChanges();
        }

        #endregion

        #region Deck Manips

        public ObservableCollection<MagicDeck> BinderDeck
        {
            get
            {
                App.database.decks.Load();
                return App.database.decks.Local.ToObservableCollection();
            }
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
            App.database.SaveChanges();
            App.state.ModifDeck();
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
            App.database.SaveChanges();
            App.state.ModifDeck();

        }

        #endregion

    }

}
