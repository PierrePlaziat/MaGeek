using MaGeek.AppData.Entities;
using MaGeek.AppFramework;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
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
            App.Events.RaisePreventUIAction(true);
            DateTime modifyTime = File.GetLastWriteTime(App.Config.Path_MtgJsonDownload);
            if (modifyTime < DateTime.Now.AddMonths(-1))
            {
                await DownloadTranslations();
                await ParseTranslations();
            }
            App.Events.RaisePreventUIAction(false);
        }

        private static async Task DownloadTranslations()
        {
            try
            {
                using var client = new HttpClient();
                using var s = await client.GetStreamAsync("https://mtgjson.com/api/v5/AllPrintings.sqlite");
                using var fs = new FileStream(App.Config.Path_MtgJsonDownload, FileMode.Create);
                await s.CopyToAsync(fs);
            }
            catch (Exception e) { MessageBoxHelper.ShowError("AddDeck", e); }
        }

        private static async Task ParseTranslations()
        {
            try
            {
                List<CardTraduction> trads = new();
                using (var connection = new SqliteConnection("Data Source="+ App.Config.Path_MtgJsonDownload))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = @" SELECT cards.name, foreign_data.language, foreign_data.name
                                             FROM cards JOIN foreign_data ON cards.uuid=foreign_data.uuid
                                             WHERE 1=1";

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            trads.Add(new CardTraduction()
                            {
                                CardId = reader.GetString(0),
                                Language = reader.GetString(1),
                                TraductedName= reader.GetString(2),
                            });
                            var name = reader.GetString(0);
                            Console.WriteLine($">>>, {name}!");
                        }
                    }
                }
                using var DB = App.DB.GetNewContext();
                {
                    foreach (var trad in trads)
                    {
                        if(!await DB.CardTraductions.Where(x=>x.CardId==trad.CardId && x.Language==trad.Language).AnyAsync())
                        {
                            DB.CardTraductions.Add(trad);
                        }
                    }
                    await DB.SaveChangesAsync();
                }
            }
            catch (Exception e) { MessageBoxHelper.ShowError("AddDeck", e); }
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
                string lang = App.Config.Settings[Setting.ForeignLangugage];
                using var DB = App.DB.GetNewContext();
                {
                    var t = await DB.CardTraductions.Where(x => x.CardId == englishName && x.Language== lang).FirstOrDefaultAsync();
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
