using Grpc.Core;
using MageekCore.Data;
using MageekCore.Services;
using MageekServer.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
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
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ConfigureEndpointDefaults(listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2;
                });


                //options.ListenAnyIP(8080, listenOptions =>
                //{
                //    listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                //});

                //options.ListenAnyIP(8081, listenOptions =>
                //{
                //    listenOptions.UseHttps("path-to-certificate.pfx", "certificate-password");
                //    listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                //});
            });

            //builder.Services.AddAuthorization();
        }
        
        public static void InitAuth(WebApplication app)
        {
            //app.UseAuthentication();
            //app.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme);
            //app.AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>("MyScheme", options => { /* configure options */ });
            //app.UseAuthorization();
            //app.UseRouting();
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
