using MageekCore.Tools;

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
        const string Url_MtgjsonPrices = "https://mtgjson.com/api/v5/AllPricesToday.json";
        const string Url_MtgjsonDecks ="https://mtgjson.com/api/v5/AllDeckFiles.zip";

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

    }

}
