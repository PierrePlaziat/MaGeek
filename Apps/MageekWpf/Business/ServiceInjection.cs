using Microsoft.Extensions.DependencyInjection;
using MageekDesktop.UI.ViewModels.AppWindows;
using MageekDesktop.UI.ViewModels.AppPanels;
using MageekDesktop.UI.Views.AppWindows;
using MageekDesktop.UI.Views.AppPanels;
using MageekDesktop.UI.ViewModels;
using MageekDesktop.UI.Views;
using MageekDesktop.UI.ViewModels.Windows;
using MageekDesktop.MageekTools.DeckTools;
using MageekClient.Services;
using MageekCore.Services;
using PlaziatWpf.Docking;
using MageekDesktop.DeckTools;

namespace MageekDesktop.Framework
{
    public static class BusinessServiceCollectionExtensions
    {

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
