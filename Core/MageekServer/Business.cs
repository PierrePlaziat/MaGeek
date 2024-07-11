using MageekCore.Data;
using MageekCore.Services;
using MageekServer.Services;
using PlaziatTools;

namespace MageekServer
{

    public static class Business
    {

        public static async Task InitBusiness(WebApplication app)
        {
            Logger.Log("Initializing...");
            var mageek = app.Services.GetService<IMageekService>();
            var initReturn = await mageek.Server_Initialize();
            if (initReturn == MageekInitReturn.Outdated)
            {
                Logger.Log("Updating...");
                _ = mageek.Server_Update().Result;
            }
            Logger.Log("Done");
            app.MapGrpcService<MageekGrpcService>();
        }

    }

}
