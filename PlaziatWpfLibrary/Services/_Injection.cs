using Microsoft.Extensions.DependencyInjection;

namespace PlaziatWpf.Services
{
    public static class FrameworkServiceCollectionExtensions
    {

        public static ServiceCollection AddPlaziatFramework(this ServiceCollection services)
        {
            services.AddSingleton<WindowsService>();
            services.AddSingleton<DialogService>();
            services.AddSingleton<SettingService>();
            services.AddSingleton<SessionBag>();
            return services;
        }

    }

}
