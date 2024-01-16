using MaGeek;
using MaGeek.UI;
using MaGeek.UI.Menus;
using MaGeek.UI.Windows.Importers;
using MageekFrontWpf.Framework.Services;
using MageekFrontWpf.UI.ViewModels;
using MageekFrontWpf.UI.ViewModels.AppPanels;
using MageekFrontWpf.UI.ViewModels.AppWindows;
using MageekFrontWpf.ViewModels;
using MageekService;
using Microsoft.Extensions.DependencyInjection;

namespace MageekFrontWpf.App
{
    public static class ServiceCollectionExtensions
    {
        public static ServiceCollection AddMageek(this ServiceCollection services)
        {
            services.AddSingleton<AppEvents>(); // TODO use mvvm messages
            services.AddSingleton<AppState>(); // TODO use mvvm messages

            // Framework
            services.AddSingleton<DialogService>();
            services.AddSingleton<SettingService>();
            services.AddSingleton<WindowsManager>();
            services.AddSingleton<CollectionImporter>();

            // Views
            services.AddSingleton<WelcomeWindow>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<ImporterUi>();
            services.AddSingleton<ImportWindow>();
            services.AddSingleton<PrecosWindow>();
            services.AddSingleton<DeckStats>();
            services.AddSingleton<CardInspector>();
            services.AddSingleton<CardSearcher>();
            services.AddSingleton<DeckContent>();
            services.AddSingleton<DeckList>();
            services.AddSingleton<DeckTable>();
            // ViewModels
            services.AddSingleton<WelcomeWindowViewModel>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<ImporterUiViewModel>();
            services.AddSingleton<ImportViewModel>();
            services.AddSingleton<PrecosViewModel>();
            services.AddSingleton<DeckStatsViewModel>();
            services.AddSingleton<CardInspectorViewModel>();
            services.AddSingleton<CardSearcherViewModel>();
            services.AddSingleton<DeckContentViewModel>();
            services.AddSingleton<DeckListViewModel>();
            services.AddSingleton<DeckTableViewModel>();
            // Menus viewModels
            services.AddSingleton<StateBarViewModel>();
            services.AddSingleton<TopMenuViewModel>();

            return services;
        }
    }
}
