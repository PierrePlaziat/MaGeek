using Grpc.Core;
using MageekCore.Data;
using MageekCore.Services;
using MageekServer.Services;
using PlaziatCore;

namespace MageekServer
{

    public static class ServerCore
    {

        public static void AddServices(WebApplicationBuilder builder)
        {
            builder.Services.AddGrpc();
            builder.Services.AddSingleton<IMageekService, MageekService>();
        }
        
        public static void InitAuth(WebApplication app)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
        }
        
        public static async Task InitBusiness(WebApplication app)
        {
            var mageek = app.Services.GetService<IMageekService>();
            var initReturn = await mageek.Server_Initialize();
            Logger.Log("MAGEEK : Initializing");
            if (initReturn == MageekInitReturn.Outdated)
            {
                Logger.Log("MAGEEK : Updating");
                _ = mageek.Server_Update().Result;
            }
            Logger.Log("MAGEEK : Ready");
            app.MapGrpcService<MageekServerService>(); // Service endpoint
        }

    }

}
