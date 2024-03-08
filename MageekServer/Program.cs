using MageekCore.Data.Collection;
using MageekCore.Data.Mtg;
using MageekServer.Services;
using MageekCore.Service;

// GRPC

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();
var app = builder.Build();
app.MapGrpcService<IdentificationService>();
app.MapGrpcService<MageekProtocolService>();
app.MapGet("/", () => "Mageek Grpc endpoint");

// MAGEEK

builder.Services.AddSingleton<IMageekService, MageekService>();

app.Run();
