using MageekSdk.Tools;
using MtgSqliveSdk.Framework;

namespace MageekSdk.MtgSqlive
{

    /// <summary>
    /// MtgSqlive Tooling for .net
    /// Call Initialize() first, then you can use GetContext()
    /// to access data through entity framework.
    /// </summary>
    public class MtgSqliveSdk
    {

        public static bool IsInitialized { get; private set; } = false;

        /// <summary>
        /// Handles retrieve of the last mtgsqlite database from web.
        /// </summary>
        /// <returns>True if the database has been updated.</returns>
        public static async Task<bool> Initialize()
        {
            Logger.Log("Start");
            try
            {
                if (IsInitialized)
                {
                    Logger.Log("Already called");
                    return false;
                }
                if (await NeedsUpdate())
                {
                    Logger.Log("Updating...");
                    await DatabaseDownload();
                    Logger.Log("Updated!");
                    HashSave();
                    IsInitialized = true;
                    return true;
                }
                else
                {
                    IsInitialized = true;
                    Logger.Log("No Update");
                    return false;
                }
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return false;
            }
        }

        /// <summary>
        /// Call this when you need to access data
        /// </summary>
        /// <returns>An entity framework context representing MtgSqlive database or null if not initialized</returns>
        public static async Task<MtgSqliveDbContext?> GetContext()
        {
            if (!IsInitialized) await Initialize();
            return IsInitialized ? new MtgSqliveDbContext(Config.Path_MtgJsonDownload) : null;
        }

        #region Methods

        private static async Task<bool> NeedsUpdate()
        {
            Logger.Log("Is update needed?");
            try
            {
                bool? tooOld = FileUtils.IsFileOlder(Config.Path_MtgJsonDownload_OldHash, new TimeSpan(0, 2, 0, 0));
                if (tooOld.HasValue && !tooOld.Value)
                {
                    Logger.Log("Already updated recently.");
                    return false;
                }
                await HashDownload();
                bool check = HashCheck();
                Logger.Log(check ? "Update available!" : "Already up to date!");
                return check;
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return false;
            }
        }
        
        private static async Task HashDownload()
        {
            if (Config.bypass_Hash)
            {
                Logger.Log("Bypassed");
                return;
            }
            try
            {
                Logger.Log("Downloading...");
                using (var client = new HttpClient())
                {
                    var hash = await client.GetStreamAsync(Config.Url_MtgjsonHash);
                    using var fs_NewHash = new FileStream(Config.Path_MtgJsonDownload_NewHash, FileMode.Create);
                    await hash.CopyToAsync(fs_NewHash);
                }
                Logger.Log("Done!");
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }

        private static bool HashCheck()
        {
            if (Config.bypass_Hash)
            {
                Logger.Log("Bypassed");
                return true;
            }
            try
            {
                Logger.Log("Checking...");
                bool check = true;
                if (File.Exists(Config.Path_MtgJsonDownload_OldHash))
                {
                    check = FileUtils.FileContentDiffers(
                        Config.Path_MtgJsonDownload_NewHash,
                        Config.Path_MtgJsonDownload_OldHash
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

        private static void HashSave()
        {
            if (Config.bypass_Hash)
            {
                Logger.Log("Bypassed");
                return;
            }
            try
            {
                Logger.Log("Copying...");
                File.Copy(Config.Path_MtgJsonDownload_NewHash, Config.Path_MtgJsonDownload_OldHash, true);
                Logger.Log("Done!");
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }

        private static async Task DatabaseDownload()
        {
            if (Config.bypass_Data)
            {
                Logger.Log("Bypassed");
                return;
            }
            try
            {
                Logger.Log("Downloading...");
                using (var client = new HttpClient())
                using (var mtgjson_sqlite = await client.GetStreamAsync(Config.Url_MtgjsonData))
                {
                    using var fs_mtgjson_sqlite = new FileStream(Config.Path_MtgJsonDownload, FileMode.Create);
                    await mtgjson_sqlite.CopyToAsync(fs_mtgjson_sqlite);
                }
                Logger.Log("Done!");
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }

        #endregion

    }

}
