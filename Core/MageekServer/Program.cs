using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MageekServer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using PlaziatIdentity;
using MageekCore.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>  options.UseSqlite(
        builder.Configuration.GetConnectionString("Data Source = " + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PlaziatIdentity\\Users.db")
    ));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
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


builder.Services.AddGrpc();

builder.WebHost.ConfigureKestrel(options => {
    options.ListenLocalhost(5000, listenOptions => {
        listenOptions.Protocols = HttpProtocols.Http2;
        listenOptions.UseHttps("C:/certificates/mageek.pfx", "pwd666");
    });
});

builder.Services.AddSingleton<IMageekService, MageekService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

await Business.InitBusiness(app);

//app.MapGrpcService<YourGrpcService>().RequireAuthorization();
app.MapGet("/", () => "Hello World!");

app.Run();