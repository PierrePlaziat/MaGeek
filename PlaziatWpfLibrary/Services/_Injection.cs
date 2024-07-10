using Microsoft.Extensions.DependencyInjection;

namespace PlaziatWpf.Services
{
    public static class FrameworkServiceCollectionExtensions
    {

        public static ServiceCollection AddFrameworkServices(this ServiceCollection services)
        {
            services.AddSingleton<WindowsService>();
            services.AddSingleton<DialogService>();
            services.AddSingleton<SettingService>();
            services.AddSingleton<SessionService>();
            return services;
        }

    }

}
