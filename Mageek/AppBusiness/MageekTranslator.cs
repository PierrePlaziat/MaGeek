using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Plaziat.CommonWpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace MaGeek.AppBusiness
{

    public static class MageekTranslator
    {

        public static async Task LoadTranslation()
        {
            await ParseTranslations();

            DateTime modifyTime = File.GetLastWriteTime(App.Config.Path_MtgJsonDownload);
            if (modifyTime < DateTime.Now.AddMonths(-1))
            {
                await DownloadTranslations();
                await ParseTranslations();
            }
        }

        private static async Task DownloadTranslations()
        {
            try
            {
                using var client = new HttpClient();
                using var s = await client.GetStreamAsync("https://mtgjson.com/api/v5/AllPrintings.json");
                using var fs = new FileStream(App.Config.Path_MtgJsonDownload, FileMode.Create);
                await s.CopyToAsync(fs);
            }
            catch (Exception e) { MessageBoxHelper.ShowError("AddDeck", e); }
        }

        private static async Task ParseTranslations()
        {
            try
            {
                using (StreamReader r = new StreamReader(App.Config.Path_MtgJsonDownload))
                {
                    string json = await r.ReadToEndAsync();
                    var data = JsonConvert.DeserializeObject<dynamic>(json);
                    // The keys you have to search through.
                    string[] keysToSearchThrough = new string[] { "card", "foreignData", };

                    string result = string.Empty;

                    // The current data it is travesing through
                    Dictionary<string, dynamic> currTraversedData = data.ToObject<Dictionary<string, dynamic>>();

                    for (int i = 0; i < keysToSearchThrough.Length; ++i)
                    {

                        if (currTraversedData.ContainsKey(keysToSearchThrough[i]))
                        {
                            // Get the value if the key current exists
                            dynamic currTravesedDataValue = currTraversedData[keysToSearchThrough[i]];

                            // Check if this 'currTravesedDataValue' can be converted to a 'Dictionary<string, dynamic>'
                            if (IsStringDynamicDictionary(currTravesedDataValue))
                            {
                                // There is still more to travese through
                                currTraversedData = currTravesedDataValue.ToObject<Dictionary<string, dynamic>>();
                            }
                            else
                            {
                                // There is no more to travese through. (This is the result)
                                result = currTravesedDataValue.ToString();

                                // TODO: Some error checking in the event that we reached the result early, even though we still have more keys to travase through
                                break;
                            }

                        }
                        else
                        {
                            // One of the keys to search through was probably invalid.
                        }
                    }

                    // ...


                }
            }
            catch (Exception e) { MessageBoxHelper.ShowError("AddDeck", e); }
        }

        static bool IsStringDynamicDictionary(dynamic input)
        {
            try
            {
                input.ToObject<Dictionary<string, dynamic>>();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<string> GetEnglishNameFromForeignName(string foreignName, string lang)
        {
            string englishName = "";
            try
            {
                using var DB = App.DB.GetNewContext();
                {
                    var t = await DB.CardTraductions.Where(x => x.TraductedName == foreignName).FirstOrDefaultAsync();
                    if(t!=null)
                    {
                        englishName = t.CardId;
                    }
                }
            }
            catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }
            return englishName;
        }

        public static async Task<string> GetTraduction(string englishName)
        {
            string foreignName = "";
            try
            {
                using var DB = App.DB.GetNewContext();
                {
                    var t = await DB.CardTraductions.Where(x => x.CardId == englishName && x.Language==App.Config.GetLang()).FirstOrDefaultAsync();
                    if (t != null)
                    {
                        foreignName = t.TraductedName;
                    }
                }
            }
            catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }
            return foreignName;
        }

    }

}
