using MtgSqliveSdk.Framework;

namespace MageekSdk.MtgSqlive
{

    public class MtgSqliveSdk
    {

        public static bool IsInitialized = false;

        public static async Task<MtgSqliveDbContext> GetContext()
        {
            if (!IsInitialized) await Initialize();
            return IsInitialized ? new MtgSqliveDbContext(Config.Path_Db) : null;
        }

        public static async Task<bool> Initialize()
        {
            Console.WriteLine("MtgSqliveSdk : Initialize");
            try
            {
                if (IsInitialized)
                {
                    Console.WriteLine("MtgSqliveSdk : Initialize > Already initialized.");
                    return false;
                }
                if (await NeedsUpdate())
                {
                    await DownloadData();
                    Console.WriteLine("MtgSqliveSdk : Initialize > Updated.");
                    IsInitialized = true;
                    return true;
                }
                else
                {
                    IsInitialized = true;
                    Console.WriteLine("MtgSqliveSdk : Initialize > No update.");
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("MtgSqliveSdk : Initialize > error :" + e.Message);
                return false;
            }
        }

        private static async Task<bool> NeedsUpdate()
        {
            Console.WriteLine("MtgSqliveSdk : NeedsUpdate");
            try
            {
                bool? tooOld = FileUtils.IsFileOlder(Config.Path_MtgJsonDownload_OldHash, new TimeSpan(3, 0, 0, 0));
                if (tooOld.HasValue && !tooOld.Value)
                {
                    Console.WriteLine("MtgSqliveSdk : NeedsUpdate > Already up to date.");
                    return false;
                }
                Console.WriteLine("MtgSqliveSdk : NeedsUpdate > Checking...");
                await HashDownload();
                bool check = await HashCheck();
                Console.WriteLine(check ? "MtgSqliveSdk : NeedsUpdate > Update available." : "MtgSqliveSdk : NeedsUpdate > Already up to date.");
                return check;
            }
            catch (Exception e)
            {
                Console.WriteLine("MtgSqliveSdk : NeedsUpdate > error : " + e.Message);
                return false;
            }
        }
        private static async Task<bool> HashCheck()
        {
            Console.WriteLine("MtgSqliveSdk : HashCheck");
            try
            {
                await Task.Delay(1);
                bool check = true;
                if (File.Exists(Config.Path_MtgJsonDownload_OldHash))
                {
                    check = FileUtils.FileContentDiffers(
                        Config.Path_MtgJsonDownload_NewHash,
                        Config.Path_MtgJsonDownload_OldHash
                    );
                }
                Console.WriteLine(check ? "MtgSqliveSdk : HashCheck > Content differs." : "MtgSqliveSdk : HashCheck > Same content.");
                return check;
            }
            catch (Exception e)
            {
                Console.WriteLine("MtgSqliveSdk : HashCheck > error : " + e.Message);
                return true;
            }
        }

        private static async Task HashDownload()
        {
            Console.WriteLine("MtgSqliveSdk : HashDownload");
            if (Config.bypass_Hash)
            {
                Console.WriteLine("MtgSqliveSdk : HashDownload > Bypassed.");
            }
            Console.WriteLine("MtgSqliveSdk : HashDownload > Downloading...");
            try
            {
                using (var client = new HttpClient())
                {
                    var hash = await client.GetStreamAsync(Config.Url_MtgjsonHash);
                    using (var fs_NewHash = new FileStream(Config.Path_MtgJsonDownload_NewHash, FileMode.Create))
                    {
                        await hash.CopyToAsync(fs_NewHash);
                    }
                }
                Console.WriteLine("MtgSqliveSdk : HashDownload > Done.");
            }
            catch (Exception e)
            {
                Console.WriteLine("MtgSqliveSdk : HashDownload > error : " + e.Message);
            }
        }

        private async static Task DownloadData()
        {
            Console.WriteLine("MtgSqliveSdk : DownloadData");
            if (Config.bypass_Data)
            {
                Console.WriteLine("MtgSqliveSdk : DownloadData > Bypassed.");
            }
            try
            {
                // File Download
                Console.WriteLine("MtgSqliveSdk : DownloadData > Downloading...");
                using (var client = new HttpClient())
                using (var mtgjson_sqlite = await client.GetStreamAsync(Config.Url_MtgjsonData))
                {
                    using var fs_mtgjson_sqlite = new FileStream(Config.Path_MtgJsonDownload, FileMode.Create);
                    await mtgjson_sqlite.CopyToAsync(fs_mtgjson_sqlite);
                }

                if (!Config.bypass_Hash)
                {
                    try
                    {
                        // Save Hash
                        Console.WriteLine("MtgSqliveSdk : DownloadData > SaveHash...");
                        File.Copy(Config.Path_MtgJsonDownload_NewHash, Config.Path_MtgJsonDownload_OldHash, true);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("MtgSqliveSdk : DownloadData > error, couldnt save hash : " + e);
                    }
                }

                Console.WriteLine("MtgSqliveSdk : DownloadData > Done.");
            }
            catch (Exception e)
            {
                Console.WriteLine("MtgSqliveSdk : DownloadData > error : " + e);
            }
        }

    }

}
