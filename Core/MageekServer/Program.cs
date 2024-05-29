using MageekCore.Data;
using MageekCore.Services;
using MageekServer.Services;
using PlaziatCore;

var builder = WebApplication.CreateBuilder(args);// Enable support for unencrypted 
builder.Services.AddGrpc();
builder.Services.AddSingleton<IMageekService, MageekService>();
var app = builder.Build();

Logger.Log("MAGEEK : Initializing");
var mageek = app.Services.GetService<IMageekService>();
var initReturn = await mageek.Server_Initialize();
if (initReturn == MageekInitReturn.Outdated)
{
    Logger.Log("MAGEEK : Updating");
    _ = mageek.Server_Update().Result;
}
Logger.Log("MAGEEK : Ready");

app.MapGrpcService<MageekServerService>();

app.MapGet("/", () => "Mageek Grpc endpoint");

app.Run();
