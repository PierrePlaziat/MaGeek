using MageekCore.Tools;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using System.IO;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MageekCore.Data.Mtg
{

    /// <summary>
    /// MtgSqlive Tooling for .net
    /// Call Initialize() first, then you can use GetContext()
    /// to access data through entity framework.
    /// </summary>
    public class MtgDbManager
    {

        const string Url_MtgjsonHash = "https://mtgjson.com/api/v5/AllPrintings.sqlite.sha256";
        const string Url_MtgjsonData ="https://mtgjson.com/api/v5/AllPrintings.sqlite";
        const string Url_MtgjsonPrecos ="https://mtgjson.com/api/v5/AllDeckFiles.zip";
        const string Url_MtgjsonPrices = "https://mtgjson.com/api/v5/AllPricesToday.json";

        public MtgDbManager()
        {
        }

        public async Task<MtgDbContext?> GetContext()
        {
            await Task.Delay(0);
            return new MtgDbContext(Folders.MtgJson_DB);
        }

        public async Task HashDownload()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var hash = await client.GetStreamAsync(Url_MtgjsonHash);
                    using var fs_NewHash = new FileStream(Folders.MtgJson_NewHash, FileMode.Create);
                    await hash.CopyToAsync(fs_NewHash);
                }
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }

        public bool HashCheck()
        {
            try
            {
                bool check = true;
                if (File.Exists(Folders.MtgJson_OldHash))
                {
                    check = FileUtils.ContentDiffers(
                        Folders.MtgJson_NewHash,
                        Folders.MtgJson_OldHash
                    );
                }
                return check;
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return true;
            }
        }

        public void HashSave()
        {
            try
            {
                File.Copy(Folders.MtgJson_NewHash, Folders.MtgJson_OldHash, true);
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }

        public async Task DatabaseDownload()
        {
            try
            {
                Logger.Log("Downloading...");
                using (var client = new HttpClient())
                using (var mtgjson_sqlite = await client.GetStreamAsync(Url_MtgjsonData))
                {
                    using var fs_mtgjson_sqlite = new FileStream(Folders.MtgJson_DB, FileMode.Create);
                    await mtgjson_sqlite.CopyToAsync(fs_mtgjson_sqlite);
                }
                Logger.Log("Done");
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }

        public async Task<bool> CheckUpdate()
        {
            try
            {
                bool? tooOld = FileUtils.IsFileOlder(Folders.MtgJson_OldHash, new TimeSpan(2, 0, 0, 0));
                if (tooOld.HasValue && !tooOld.Value)
                {
                    Logger.Log("Already updated recently");
                    return false;
                }
                Logger.Log("Checking...");
                await HashDownload();
                bool check = HashCheck();
                Logger.Log(check ? "Update available" : "Already up to date");
                return check;
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return false;
            }
        }

        public async Task RetrievePrecos()
        {
            string tmpPath = Path.Combine(Folders.PrecosFolder,"temp");
            if (!Directory.Exists(tmpPath)) Directory.CreateDirectory(tmpPath);
            string zipPath = Path.Combine(tmpPath, "precos.zip");
            Logger.Log("Downloading...");
            using (var client = new HttpClient())
            using (var precosZip = await client.GetStreamAsync(Url_MtgjsonPrecos))
            {
                using var fs_PrecosZip = new FileStream(zipPath, FileMode.Create);
                await precosZip.CopyToAsync(fs_PrecosZip);
            }
            Logger.Log("Uncompressing...");
            System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, tmpPath, overwriteFiles: true);
            Logger.Log("Parsing...");
            await ParsePrecos(tmpPath, Folders.PrecosFolder);
            Logger.Log("Cleaning");
            Directory.Delete(tmpPath, true);
            Logger.Log("Done");
        }

        public async Task ParsePrecos(string tmpPath, string precosFolder)
        {
            // READ
            List<Preco> list = new List<Preco>();
            foreach (string precoPath in Directory.GetFiles(tmpPath))
            {
                try
                {
                    list.Add(await ParsePreco(precoPath));
                }
                catch (Exception e) { Logger.Log(e); }
            }
            // WRITE
            Console.WriteLine(DateTime.Now);
            var options = new JsonSerializerOptions { IncludeFields=true};
            string jsonString = JsonSerializer.Serialize(list, options);
            File.WriteAllText(Path.Combine(Folders.PrecosFolder, "precos.json"), jsonString);

        }

        private async Task<Preco> ParsePreco(string precoPath)
        {
            using StreamReader reader = new(precoPath);
            string jsonString = await reader.ReadToEndAsync();
            dynamic dynData = JObject.Parse(jsonString);

            string code = dynData.data.code;
            string name = dynData.data.name;
            string releaseDate = dynData.data.releaseDate;
            string type = dynData.data.type;

            List<Tuple<string, int>> CommanderCardUuids = new List<Tuple<string, int>>();
            foreach(dynamic card in dynData.data.commander)
            {
                string uuid = card.uuid;
                int quantity = card.count;
                CommanderCardUuids.Add(new Tuple<string,int>(uuid, quantity));
            }
            
            List<Tuple<string, int>> mainCardUuids = new List<Tuple<string, int>>();
            foreach(dynamic card in dynData.data.mainBoard)
            {
                string uuid = card.uuid;
                int quantity = card.count;
                mainCardUuids.Add(new Tuple<string, int>(uuid, quantity));
            }

            List<Tuple<string, int>> sideCardUuids = new List<Tuple<string, int>>();
            foreach (dynamic card in dynData.data.sideBoard)
            {
                string uuid = card.uuid;
                int quantity = card.count;
                sideCardUuids.Add(new Tuple<string, int>(uuid, quantity));
            }

            return new Preco()
            {
                Title = name,
                Code = code,
                ReleaseDate = releaseDate,
                Kind = type,
                CommanderCardUuids = CommanderCardUuids,
                MainCardUuids = mainCardUuids,
                SideCardUuids = sideCardUuids
            };

        }
    
    }

}
