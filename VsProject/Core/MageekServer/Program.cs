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
using PlaziatTools;

Logger.Log("[CREATE]");
var builder = WebApplication.CreateBuilder(args);

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

Logger.Log("InitFolders");
PlaziatTools.Paths.Init();

Logger.Log("Add Grpc");
builder.Services.AddGrpc(options => { options.EnableDetailedErrors = true; });

Logger.Log("Creating new certificate");
var (certificatePath, password) = CertificateGenerator.GenerateSelfSignedCertificate("Certificate.pfx");
builder.WebHost.ConfigureKestrel(options => {
    options.ListenLocalhost(5000, listenOptions => {
        listenOptions.Protocols = HttpProtocols.Http2;
        listenOptions.UseHttps(certificatePath, password);
    });
});

Logger.Log("Add Identity Framework");
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

Logger.Log("Add Mageek");
builder.Services.AddSingleton<IMageekService, MageekService>();

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

Logger.Log("[BUILD]");
var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

Logger.Log("Init DB");
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<UsersDbContext>();
    dbContext.Database.EnsureCreated();
    //dbContext.Database.Migrate();
}
await dbConn.CloseAsync();

Logger.Log("Init MaGeek");
var mageek = app.Services.GetService<IMageekService>();
var initReturn = await mageek.Server_Initialize();
if (initReturn == MageekInitReturn.Outdated) _ = mageek.Server_Update().Result;

Logger.Log("[RUN]");
app.MapGrpcService<MageekGrpcService>();
app.Run();
Logger.Log("[[[ RUNNING ]]]");

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
