using MtgApiManager.Lib.Core;
using MtgApiManager.Lib.Model;
using MtgApiManager.Lib.Service;
using Plaziat.CommonWpf;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaGeek.Data
{

    public class MTG_API
    {

        IMtgServiceProvider serviceProvider = new MtgServiceProvider();

        public async Task<List<ICard>> SearchCards(string searchText)
        {
            ICardService service = serviceProvider.GetCardService();
            IOperationResult<List<ICard>> tmpResult = null;
            List<ICard> cards = new List<ICard>();
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

    }

}
