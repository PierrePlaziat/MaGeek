using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using PlaziatIdentity;
using MageekCore.Services;
using Microsoft.Data.Sqlite;
using MageekCore.Data;
using MageekServer.Services;

var builder = WebApplication.CreateBuilder(args);

//////////
// INIT //
//////////

PlaziatTools.Paths.Init();

// Grpc 
// ssl http2 sole endpoint
builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
});
builder.WebHost.ConfigureKestrel(options => {
    options.ListenLocalhost(5000, listenOptions => {
        listenOptions.Protocols = HttpProtocols.Http2;
        listenOptions.UseHttps("C:/certificates/mageek.pfx", "pwd666");
    });
});
// Identity framework
// over ef sqlite with jwt auth
using var dbConn = new SqliteConnection("Data Source = " + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MageekServer\\Users.db");
await dbConn.OpenAsync();
builder.Services.AddDbContext<UsersDbContext>(options =>  options.UseSqlite(dbConn));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<UsersDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
builder.Services.AddAuthorization(options => {
        options.AddPolicy("AdminPolicy", policy => policy.RequireClaim("AdminClaim", "Admin"));
    });
builder.Services.AddScoped<IUserService, UserService>();
// Business service
builder.Services.AddSingleton<IMageekService, MageekService>();

////////////
// Launch //
////////////

// Build
var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
// Db migration
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<UsersDbContext>();
    dbContext.Database.EnsureCreated();
    //dbContext.Database.Migrate();
}
await dbConn.CloseAsync();
// Business
var mageek = app.Services.GetService<IMageekService>();
var initReturn = await mageek.Server_Initialize();
if (initReturn == MageekInitReturn.Outdated) _ = mageek.Server_Update().Result;
// Go
app.MapGrpcService<MageekGrpcService>();
app.Run();
