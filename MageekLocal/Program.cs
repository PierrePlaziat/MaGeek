using MageekServer;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args); // Https support
ServerCore.AddServices(builder);
var app = builder.Build();
ServerCore.InitAuth(app);
await ServerCore.InitBusiness(app);

// Run
app.MapGet("/", () => ""); // Navigator endpoint
app.Run();
