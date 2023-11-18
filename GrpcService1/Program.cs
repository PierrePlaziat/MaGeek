using MageekGrpc.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();
var app = builder.Build();

app.MapGrpcService<GreeterService>();
app.MapGrpcService<CollectionnerService>();
app.MapGrpcService<TaggerService>();
app.MapGrpcService<DeckBuilderService>();
app.MapGet("/", () => "Mageek Grpc endpoint");

app.Run();
