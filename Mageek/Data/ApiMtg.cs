using MaGeek.Data.Entities;
using MtgApiManager.Lib.Core;
using MtgApiManager.Lib.Model;
using MtgApiManager.Lib.Service;
using Plaziat.CommonWpf;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaGeek.Data
{

    public class ApiMtg
    {

        readonly IMtgServiceProvider serviceProvider = new MtgServiceProvider();

        public async Task<List<ICard>> SearchCards(string searchText)
        {
            ICardService service = serviceProvider.GetCardService();
            IOperationResult<List<ICard>> tmpResult = null;
            List<ICard> cards = new();
            int i = 1;
            do
            {
                tmpResult = await service
                    .Where(x => x.Name, searchText)
                    .Where(x => x.Page, i)
                    .Where(x => x.PageSize, 100)
                    .AllAsync();
                if (tmpResult.IsSuccess)
                {
                    cards.AddRange(tmpResult.Value);
                }
                else
                {
                    var exception = tmpResult.Exception;
                    MessageBoxHelper.ShowMsg(exception.Message);
                }
                i++;
            }
            while (tmpResult.Value.Count>0);
            return cards;
        }

        public async Task SearchCardsOnline(string cardname, bool onlyOne = false)
        {
            var foundCards = await SearchCards(cardname);
            foreach (var foundCard in foundCards)
            {
                
                if (!onlyOne || (foundCard != null && NameCorresponds(foundCard.Name,cardname)))
                {
                    MagicCard card = SaveLocalCard(foundCard);
                    card.AddVariant(foundCard);
                }
                App.database.SaveChanges();
            }
        }

        private static bool NameCorresponds(string name, string cardname)
        {
            string[] ss = name.Split(" // ");
            foreach (string ss2 in ss)
            {
                if (ss2 == cardname ) return true;
            }
            return false;
        }

        private static MagicCard SaveLocalCard(ICard iCard)
        {
            // Guard
            var card = FindLocalCard(iCard);
            if (card != null) return card;
            // Proceed
            card = new MagicCard(iCard);
            App.database.cards.Add(card); //App.Current.Dispatcher.Invoke(delegate { App.database.cards.Add(card); });
            return card;
        }

        private static MagicCard FindLocalCard(ICard iCard)
        {
            return App.database.cards.Where(x => x.CardId== iCard.Name).FirstOrDefault();
        }

    }

}
