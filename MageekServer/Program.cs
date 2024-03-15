using MageekCore.Service;
using MageekProtocol;

// GRPC

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();
var app = builder.Build();
app.MapGrpcService< MageekProtocolService>();
app.MapGet("/", () => "Mageek Grpc endpoint");

// MAGEEK

builder.Services.AddSingleton<IMageekService, MageekService>();

app.Run();
