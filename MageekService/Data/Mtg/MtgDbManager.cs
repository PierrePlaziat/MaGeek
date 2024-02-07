using MageekServices.Data.Collection;
using MageekServices.Tools;
using Microsoft.Extensions.Logging;
using System.Configuration;

namespace MageekServices.Data.Mtg
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

        private readonly ILogger<MtgDbManager> logger;

        public MtgDbManager(ILogger<MtgDbManager> logger)
        {
            this.logger = logger;
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
                logger.LogInformation("Downloading...");
                using (var client = new HttpClient())
                {
                    var hash = await client.GetStreamAsync(Url_MtgjsonHash);
                    using var fs_NewHash = new FileStream(Folders.MtgJson_NewHash, FileMode.Create);
                    await hash.CopyToAsync(fs_NewHash);
                }
                logger.LogInformation("Done!");
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
        }

        public bool HashCheck()
        {
            try
            {
                logger.LogTrace("Checking...");
                bool check = true;
                if (File.Exists(Folders.MtgJson_OldHash))
                {
                    check = FileUtils.FileContentDiffers(
                        Folders.MtgJson_NewHash,
                        Folders.MtgJson_OldHash
                    );
                }
                logger.LogInformation("Done!");
                return check;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return true;
            }
        }

        public void HashSave()
        {
            try
            {
                logger.LogInformation("Copying...");
                File.Copy(Folders.MtgJson_NewHash, Folders.MtgJson_OldHash, true);
                logger.LogInformation("Done!");
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
        }

        public async Task DatabaseDownload()
        {
            try
            {
                logger.LogInformation("Downloading...");
                using (var client = new HttpClient())
                using (var mtgjson_sqlite = await client.GetStreamAsync(Url_MtgjsonData))
                {
                    using var fs_mtgjson_sqlite = new FileStream(Folders.MtgJson_DB, FileMode.Create);
                    await mtgjson_sqlite.CopyToAsync(fs_mtgjson_sqlite);
                }
                logger.LogInformation("Done!");
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
        }

        public async Task<bool> UpdateAvailable()
        {
            logger.LogInformation("Is update needed?");
            try
            {
                bool? tooOld = FileUtils.IsFileOlder(Folders.MtgJson_OldHash, new TimeSpan(2, 0, 0, 0));
                if (tooOld.HasValue && !tooOld.Value)
                {
                    logger.LogInformation("Already updated recently.");
                    return false;
                }
                await HashDownload();
                bool check = HashCheck();
                logger.LogInformation(check ? "Update available!" : "Already up to date!");
                return check;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return false;
            }
        }

    }

}
