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

        public List<ICard> GetCardsByName(string cardName)
        {
            ICardService service = serviceProvider.GetCardService();
            IOperationResult<List<ICard>> result = service.Where(x => x.ForeignName, cardName).AllAsync().Result;
            if (result.IsSuccess)
            {
                var value = result.Value;
            }
            else
            {
                var exception = result.Exception;
            }
            return result.Value;
        }
        public async Task<List<ICard>> Get100CardsByName_Async(string cardName)
        {
            ICardService service = serviceProvider.GetCardService();
            var result = await service.Where(x => x.Name, cardName).AllAsync();
            if (result.IsSuccess)
            {
                var value = result.Value;
            }
            else
            {
                var exception = result.Exception;
                MessageBoxHelper.ShowMsg(exception.Message);
            }
            return result.Value;
        }

        public async Task<List<ICard>> GetAllCardsByName_Async(string cardName)
        {
            ICardService service = serviceProvider.GetCardService();
            IOperationResult<List<ICard>> tmpResult = null;
            List<ICard> cards = new List<ICard>();
            int i = 1;
            do
            {
                tmpResult = await service.Where(x => x.Name, cardName)
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
