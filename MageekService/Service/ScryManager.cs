using MageekCore.Data;
using MageekCore.Data.Mtg;
using MageekCore.Data.Mtg.Entities;
using Microsoft.EntityFrameworkCore;
using PlaziatTools;
using ScryfallApi.Client.Models;
using System.Text.Json;

namespace MageekCore.Service
{

    internal class ScryManager
    {

        private MtgDbManager mtg;

        public ScryManager(MtgDbManager mtg)
        {
            this.mtg = mtg;
        }

        /// <summary>
        /// This will disappear when using mtgsqlive data,
        /// get card data from scryfall from a card uuid
        /// </summary>
        /// <param name="cardUuid"></param>
        /// <returns>A scryfall card</returns>
        public async Task<Card?> GetScryfallCard(string cardUuid)
        {
            Logger.Log("");
            try
            {
                using MtgDbContext DB2 = await mtg.GetContext();
                var v = DB2.cardIdentifiers.Where(x => x.Uuid == cardUuid).FirstOrDefault();
                if (v == null) return null;
                string? scryfallId = v.ScryfallId;
                if (scryfallId == null) return null;
                Thread.Sleep(150);
                string json_data = await HttpUtils.Get("https://api.scryfall.com/cards/" + scryfallId);
                Card scryfallCard = JsonSerializer.Deserialize<Card>(json_data);
                return scryfallCard;
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return null;
            }
        }

        public async Task DownloadImage(string cardUuid, CardImageFormat type, string localFileName)
        {
            var scryData = await GetScryfallCard(cardUuid);
            var httpClient = new HttpClient();
            var uri = scryData.ImageUris[type.ToString()];
            using var stream = await httpClient.GetStreamAsync(uri);
            using var fileStream = new FileStream(localFileName, FileMode.Create);
            await stream.CopyToAsync(fileStream);
        }

        public async Task<ResultList<Set>> GetSetsJson()
        {
            string json_data = await HttpUtils.Get("https://api.scryfall.com/sets/");
            var sets = JsonSerializer.Deserialize<ResultList<Set>>(json_data);
            return sets;
        }

        public async Task FetchSets()
        {
            Logger.Log("Start");
            try
            {
                var sets = await GetSetsJson();
                using (HttpClient client = new HttpClient())
                {
                    foreach (Set set in sets.Data)
                    {
                        var uri = set.IconSvgUri;
                        string localFileName = Path.Combine(Folders.SetIcon, set.Code.ToUpper() + "_.svg");
                        if (!File.Exists(localFileName))
                        {
                            using (var s = await client.GetStreamAsync(uri))
                            {
                                try
                                {
                                    using (var fs = new FileStream(localFileName, FileMode.OpenOrCreate))
                                    {
                                        await s.CopyToAsync(fs);
                                    }
                                }
                                catch (Exception e) { Logger.Log(e); }
                            }
                        }
                    }
                }
            }
            catch (Exception e) { Logger.Log(e); }
            Logger.Log("Done");
        }

    }

}
