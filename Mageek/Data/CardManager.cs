using MaGeek.Data.Entities;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;

namespace MaGeek.Data
{

    public class CardManager
    {

        public ApiMtg MtgApi = new ApiMtg();

        public void AddCardToDeck(MagicCard card, MagicDeck deck)
        {
            deck.Cards.Add(card);
            App.database.SaveChanges();
        }

        public void GotCard_Add(MagicCard selectedCard)
        {
            App.database.cards.Where(x => x.Name_VO == selectedCard.Name_VO).FirstOrDefault().CollectedQuantity++;
            App.database.SaveChanges();
        }

        public void GotCard_Remove(MagicCard selectedCard)
        {
            App.database.cards.Where(x => x.Name_VO == selectedCard.Name_VO).FirstOrDefault().CollectedQuantity--;
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
