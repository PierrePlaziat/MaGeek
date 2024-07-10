using CommunityToolkit.Maui;
using MageekClient.Services;
using MageekCore.Services;
using MageekMobile.ViewModels;
using MageekMobile.Views;
using Microsoft.Extensions.Logging;

namespace MageekMobile
{
    public static class MauiProgram
    {

        public static MauiApp CreateMauiApp()
        {

            var builder = MauiApp.CreateBuilder()
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<IMageekService, MageekClientService>();
            builder.Services.AddSingleton<INavigationService, NavigationService>();

            builder.Services.AddSingleton<WelcomeView>();
            builder.Services.AddSingleton<DeckView>();
            builder.Services.AddSingleton<CollecView>();
            builder.Services.AddSingleton<GameView>();

            builder.Services.AddSingleton<WelcomeViewModel>();
            builder.Services.AddSingleton<DeckViewModel>();
            builder.Services.AddSingleton<CollecViewModel>();
            builder.Services.AddSingleton<GameViewModel>();

            var app = builder.Build();
            //ServiceHelper.Initialize(app.Services);
            return app;
        }
    }

    //public static class ServiceHelper
    //{
    //    public static IServiceProvider Services { get; private set; }
    //    public static void Initialize(IServiceProvider services) => Services = services;
    //    public static T GetService<T>() => Services.GetService<T>();
    //}

}
