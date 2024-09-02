using PlaziatTools;
using ScryfallApi.Client.Models;
using System.Text.Json;

namespace MageekCore.Data
{

    public class ScryfallHelper
    {

        public async Task<Card?> GetScryfallCard(string scryfallId)
        {
            Logger.Log("");
            try
            {
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

        public async Task CacheIllustration(string scryfallId, CardImageFormat format, string localFileName, bool back = false)
        {
            Logger.Log("");
            try
            {
                // Get scryfall Card data
                if (scryfallId == null) return;
                Thread.Sleep(150);
                string json_data = await HttpUtils.Get("https://api.scryfall.com/cards/" + scryfallId);
                Card scryData = JsonSerializer.Deserialize<Card>(json_data);
                // Retrieve Image link
                Uri uri;
                if (scryData.ImageUris != null) uri = scryData.ImageUris[format.ToString()];
                else uri = scryData.CardFaces[back ? 1 : 0].ImageUris[format.ToString()];
                // Download it
                var httpClient = new HttpClient();
                using var stream = await httpClient.GetStreamAsync(uri);
                using var fileStream = new FileStream(localFileName, FileMode.Create);
                await stream.CopyToAsync(fileStream);
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return;
            }
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
                        string localFileName = Path.Combine(Paths.Folder_SetIcons, set.Code.ToUpper() + "_.svg");
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
                                catch (Exception e) 
                                { 
                                    Logger.Log(e);
                                    Logger.Log(uri.ToString());
                                }
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
