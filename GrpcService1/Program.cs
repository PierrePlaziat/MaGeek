using MageekGrpc.Services;

await MageekService.MageekService.InitializeService();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();
var app = builder.Build();

app.MapGrpcService<GreeterService>();
app.MapGrpcService<CollectionnerService>();
app.MapGet("/", () => "Mageek Grpc endpoint");

app.Run();
