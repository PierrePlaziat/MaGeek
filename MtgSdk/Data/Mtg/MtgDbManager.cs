using MageekSdk.Tools;
using System.Configuration;

namespace MageekSdk.Data.Mtg
{

    /// <summary>
    /// MtgSqlive Tooling for .net
    /// Call Initialize() first, then you can use GetContext()
    /// to access data through entity framework.
    /// </summary>
    public static class MtgDbManager
    {

        public static async Task<MtgDbContext?> GetContext()
        {
            await Task.Delay(0);
            return new MtgDbContext(MageekFolders.MtgJson_DB);
        }

        public static async Task HashDownload()
        {
            try
            {
                Logger.Log("Downloading...");
                using (var client = new HttpClient())
                {
                    var hash = await client.GetStreamAsync(ConfigurationManager.AppSettings["Url_MtgjsonHash"]);
                    using var fs_NewHash = new FileStream(MageekFolders.MtgJson_NewHash, FileMode.Create);
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
                if (File.Exists(MageekFolders.MtgJson_OldHash))
                {
                    check = FileUtils.FileContentDiffers(
                        MageekFolders.MtgJson_NewHash,
                        MageekFolders.MtgJson_OldHash
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
                File.Copy(MageekFolders.MtgJson_NewHash, MageekFolders.MtgJson_OldHash, true);
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
                using (var mtgjson_sqlite = await client.GetStreamAsync(ConfigurationManager.AppSettings["Url_MtgjsonData"]))
                {
                    using var fs_mtgjson_sqlite = new FileStream(MageekFolders.MtgJson_DB, FileMode.Create);
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
                bool? tooOld = FileUtils.IsFileOlder(MageekFolders.MtgJson_OldHash, new TimeSpan(2, 0, 0, 0));
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
