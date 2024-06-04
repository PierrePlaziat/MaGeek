using Microsoft.Extensions.DependencyInjection;
using MageekFrontWpf.UI.ViewModels.AppWindows;
using MageekFrontWpf.UI.ViewModels.AppPanels;
using MageekFrontWpf.UI.Views.AppWindows;
using MageekFrontWpf.UI.Views.AppPanels;
using MageekFrontWpf.UI.ViewModels;
using MageekFrontWpf.UI.Views;
using MageekFrontWpf.UI.ViewModels.Windows;
using PlaziatWpf.Services;
using MageekFrontWpf.MageekTools.DeckTools;
using MageekClient.Services;
using MageekCore.Services;
using PlaziatWpf.Docking;
using MageekFrontWpf.DeckTools;

namespace MageekFrontWpf.Framework
{
    public static class ServiceCollectionExtensions
    {

        public static ServiceCollection AddFrameworkServices(this ServiceCollection services)
        {
            services.AddSingleton<WindowsService>();
            services.AddSingleton<DialogService>();
            services.AddSingleton<SettingService>();
            services.AddSingleton<SessionService>();
            return services;
        }

        public static ServiceCollection AddMageek(this ServiceCollection services)
        {
            services.AddSingleton<IMageekService, MageekClientService>();
            services.AddSingleton<DeckManipulator>();

            // Views //

            // Windows
            services.AddSingleton<WelcomeWindow>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<PrintWindow>();
            // Tools
            services.AddSingleton<CardInspector>();
            services.AddSingleton<CardSearcher>();
            services.AddSingleton<PrecoList>();
            services.AddSingleton<DeckList>();
            services.AddSingleton<SetList>();
            services.AddSingleton<CollecEstimation>();
            services.AddSingleton<TxtInput>();
            // Documents
            services.AddTransient<IDocument, Document>();
            services.AddTransient<ManipulableDeck>();

            // ViewModels //

            // Windows
            services.AddSingleton<WelcomeWindowViewModel>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<PrintWindowViewModel>();
            // Tools
            services.AddSingleton<CardInspectorViewModel>();
            services.AddSingleton<CardSearcherViewModel>();
            services.AddSingleton<PrecoListViewModel>();
            services.AddSingleton<DeckListViewModel>();
            services.AddSingleton<SetListViewModel>();
            services.AddSingleton<CollecEstimationViewModel>();
            services.AddSingleton<TxtInputViewModel>();
            // Documents
            services.AddTransient<DocumentViewModel>();
            // Top menu
            services.AddSingleton<TopMenuViewModel>();

            return services;
        }

    }

}
