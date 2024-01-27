using Microsoft.Extensions.DependencyInjection;
using MageekService;
using MageekFrontWpf.Framework.Services;
using MageekFrontWpf.UI.ViewModels.AppWindows;
using MageekFrontWpf.UI.ViewModels.AppPanels;
using MageekFrontWpf.UI.Views.AppWindows;
using MageekFrontWpf.UI.Views.AppPanels;
using MageekFrontWpf.UI.ViewModels;

namespace MageekFrontWpf.AppValues
{
    public static class ServiceCollectionExtensions
    {

        public static ServiceCollection AddMageek(this ServiceCollection services)
        {

            // Framework
            services.AddSingleton<DialogService>();
            services.AddSingleton<SettingService>();
            services.AddSingleton<WindowsService>();
            services.AddSingleton<CollectionImporter>();
            services.AddSingleton<TopMenuViewModel>();

            // Views
            services.AddSingleton<WelcomeWindow>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<ImportWindow>();
            services.AddSingleton<PrecosWindow>();
            services.AddSingleton<DeckStats>();
            services.AddSingleton<CardInspector>();
            services.AddSingleton<CardSearcher>();
            services.AddSingleton<DeckContent>();
            services.AddSingleton<DeckList>();
            services.AddSingleton<DeckTable>();
            services.AddSingleton<ImporterUi>();
            // ViewModels
            services.AddSingleton<WelcomeWindowViewModel>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<ImportViewModel>();
            services.AddSingleton<PrecosViewModel>();
            services.AddSingleton<DeckStatsViewModel>();
            services.AddSingleton<CardInspectorViewModel>();
            services.AddSingleton<CardSearcherViewModel>();
            services.AddSingleton<DeckContentViewModel>();
            services.AddSingleton<DeckListViewModel>();
            services.AddSingleton<DeckTableViewModel>();
            services.AddSingleton<ImporterUiViewModel>();

            return services;
        }
    }
}
