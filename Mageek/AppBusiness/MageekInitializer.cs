using MaGeek.AppData.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Plaziat.CommonWpf;
using ScryfallApi.Client.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace MaGeek.AppBusiness
{

    public static class MageekInitializer
    {

        public static async Task Initialize()
        {
            App.Events.RaisePreventUIAction(true,"");
            await ParseCards();
            DateTime modifyTime = File.GetLastWriteTime(App.Config.Path_MtgJsonDownload);
            if (modifyTime < DateTime.Now.AddMonths(-1))
            {
                App.Events.RaisePreventUIAction(true, "First launch, 1/3 : Downloading data");
                await Task.Delay(100);
                await DownloadMtgJsonSqlite();
                App.Events.RaisePreventUIAction(true, "First launch, 2/3 : Importing translations");
                await Task.Delay(100);
                await ParseTranslations();
                App.Events.RaisePreventUIAction(true, "First launch, 3/3 : Importing cards");
                await Task.Delay(100);
                await ParseCards();
            }
            App.Events.RaisePreventUIAction(false,"");
        }

        private static async Task DownloadMtgJsonSqlite()
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

        private static async Task ParseCards()
        {
            await Task.Run(async () => {
                try
                {
                    List<Card> scryCards = new();
                    using (var connection = new SqliteConnection("Data Source=" + App.Config.Path_MtgJsonDownload))
                    {
                        connection.Open();

                        var command = connection.CreateCommand();
                        command.CommandText = @" SELECT 
                                                    cards.name, 
                                                    cards.type, 
                                                    cards.text,
                                                    cards.keywords,
                                                    cards.power,
                                                    cards.toughness,
                                                    cards.manaCost,
                                                    cards.convertedManaCost,
                                                    cards.colorIdentity,
                                                    cards.scryfallId,
                                                    cards.rarity,
                                                    cards.artist,
                                                    cards.language,
                                                    sets.name
                                                 FROM cards JOIN sets ON cards.setCode=sets.code
                                                 WHERE 1=1";

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var v0 = reader.GetString(0);
                                var v1 = reader.GetString(1);

                                string v2tmp = "";
                                if (!await reader.IsDBNullAsync(2)) v2tmp = reader.GetString(2);
                                var v2 = v2tmp;

                                string v3tmp = "";
                                if (!await reader.IsDBNullAsync(3)) v3tmp = reader.GetString(3);
                                var v3 = new string[] { v3tmp };

                                string v4tmp = "";
                                if (!await reader.IsDBNullAsync(4)) v4tmp = reader.GetString(4);
                                var v4 = v4tmp;

                                string v5tmp = "";
                                if (!await reader.IsDBNullAsync(5)) v5tmp = reader.GetString(5);
                                var v5 = v5tmp;

                                string v6tmp = "";
                                if (!await reader.IsDBNullAsync(6)) v6tmp = reader.GetString(6);
                                var v6 = v6tmp;

                                var v7 = decimal.Parse(reader.GetFloat(7).ToString());

                                string v8tmp = "";
                                if (!await reader.IsDBNullAsync(8)) v8tmp = reader.GetString(8);
                                var v8 = new string[] { v8tmp };

                                var v9 = Guid.Parse(reader.GetString(9));

                                var v10 = reader.GetString(10);

                                string v11tmp = "";
                                if (!await reader.IsDBNullAsync(11)) v11tmp = reader.GetString(11);
                                var v11 = v11tmp;

                                var v12 = reader.GetString(12);
                            
                                var v13 = reader.GetString(13);

                                scryCards.Add(new Card()
                                {
                                    // Card
                                    Name = v0,
                                    TypeLine = v1,
                                    OracleText = v2,
                                    Keywords = v3,
                                    Power = v4,
                                    Toughness = v5,
                                    ManaCost = v6,
                                    Cmc = v7,
                                    ColorIdentity = v8,
                                    // Variant
                                    Id = v9,
                                    Rarity = v10,
                                    Artist = v11,
                                    Language = v12,
                                    SetName = v13,
                                });
                            }
                        }
                    }
                    await MageekApi.RecordCards2(scryCards);
                    App.Events.RaiseUpdateCardCollec();
                }
                catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }

            });
        }

        private static async Task ParseTranslations()
        {
            await Task.Run(async () => {
                try
                {
                    List<CardTraduction> trads = new();
                    using (var connection = new SqliteConnection("Data Source="+ App.Config.Path_MtgJsonDownload))
                    {
                        await connection.OpenAsync();

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
                catch (Exception e) { MessageBoxHelper.ShowError(MethodBase.GetCurrentMethod().Name, e); }
            });
        }

    }

}
