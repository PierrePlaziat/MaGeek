using MageekCore.Data.Collection;
using MageekCore.Data.Mtg;
using MageekCore;
using MageekServer.Services;

// GRPC

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();
var app = builder.Build();
app.MapGrpcService<GreeterService>();
app.MapGrpcService<CollectionnerService>();
app.MapGet("/", () => "Mageek Grpc endpoint");

// MAGEEK
builder.Services.AddSingleton<CollectionDbManager>();
builder.Services.AddSingleton<MtgDbManager>();
builder.Services.AddSingleton<MageekService>();

app.Run();
