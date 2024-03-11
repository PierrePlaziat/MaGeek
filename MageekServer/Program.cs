using MageekServer.Services;
using MageekCore.Service;

// GRPC

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();
var app = builder.Build();
app.MapGrpcService<CollectionnerService>();
app.MapGet("/", () => "Mageek Grpc endpoint");

// MAGEEK

builder.Services.AddSingleton<IMageekService, MageekService>();

app.Run();
