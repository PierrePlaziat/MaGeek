using MageekCore.Data;
using MageekCore.Services;
using MageekServer.Services;
using PlaziatIdentity;
using PlaziatTools;

namespace MageekServer
{

    public static class ServerCore
    {

        public static void AddServices(WebApplicationBuilder builder)
        {
            builder.Services.AddGrpc();
            builder.Services.AddSingleton<IMageekService, MageekService>();
            builder.Services.AddSingleton<IUserService, UserService>();
        }
        
        public static void InitAuth(WebApplication app)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
        }
        
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
