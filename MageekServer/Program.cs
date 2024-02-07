using MageekServer.Services;

// GRPC
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();
var app = builder.Build();
app.MapGrpcService<GreeterService>();
app.MapGrpcService<CollectionnerService>();
app.MapGet("/", () => "Mageek Grpc endpoint");

// MAGEEK
//await MageekService.MageekService.InitializeService();

app.Run();
