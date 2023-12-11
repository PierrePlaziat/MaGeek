using MageekService.Tools;
using System.Configuration;

namespace MageekService.Data.Mtg
{

    /// <summary>
    /// MtgSqlive Tooling for .net
    /// Call Initialize() first, then you can use GetContext()
    /// to access data through entity framework.
    /// </summary>
    public static class MtgDbManager
    {

        static string Url_MtgjsonHash = "https://mtgjson.com/api/v5/AllPrintings.sqlite.sha256";
        static string Url_MtgjsonData ="https://mtgjson.com/api/v5/AllPrintings.sqlite";
        static string Url_MtgjsonPrices = "https://mtgjson.com/api/v5/AllPricesToday.json";
        static string Url_MtgjsonDecks ="https://mtgjson.com/api/v5/AllDeckFiles.zip";
    

        public static async Task<MtgDbContext?> GetContext()
        {
            await Task.Delay(0);
            return new MtgDbContext(Folders.MtgJson_DB);
        }

        public static async Task HashDownload()
        {
            try
            {
                Logger.Log("Downloading...");
                using (var client = new HttpClient())
                {
                    var hash = await client.GetStreamAsync(Url_MtgjsonHash);
                    using var fs_NewHash = new FileStream(Folders.MtgJson_NewHash, FileMode.Create);
                    await hash.CopyToAsync(fs_NewHash);
                }
                Logger.Log("Done!");
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }

        public static bool HashCheck()
        {
            try
            {
                Logger.Log("Checking...");
                bool check = true;
                if (File.Exists(Folders.MtgJson_OldHash))
                {
                    check = FileUtils.FileContentDiffers(
                        Folders.MtgJson_NewHash,
                        Folders.MtgJson_OldHash
                    );
                }
                Logger.Log("Done!");
                return check;
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return true;
            }
        }

        public static void HashSave()
        {
            try
            {
                Logger.Log("Copying...");
                File.Copy(Folders.MtgJson_NewHash, Folders.MtgJson_OldHash, true);
                Logger.Log("Done!");
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }

        public static async Task DatabaseDownload()
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
                Logger.Log("Done!");
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }

        public static async Task<bool> NeedsUpdate()
        {
            Logger.Log("Is update needed?");
            try
            {
                bool? tooOld = FileUtils.IsFileOlder(Folders.MtgJson_OldHash, new TimeSpan(2, 0, 0, 0));
                if (tooOld.HasValue && !tooOld.Value)
                {
                    Logger.Log("Already updated recently.");
                    return false;
                }
                await MtgDbManager.HashDownload();
                bool check = MtgDbManager.HashCheck();
                Logger.Log(check ? "Update available!" : "Already up to date!");
                return check;
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return false;
            }
        }

    }

}
