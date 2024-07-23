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

        public static ServiceCollection AddBusiness(this ServiceCollection services)
        {
            services.AddSingleton<IMageekService, MageekClientService>();
            services.AddSingleton<DeckManipulator>();
            return services;
        }

        public static ServiceCollection AddViews(this ServiceCollection services)
        {
            // Windows
            services.AddSingleton<WelcomeWindow>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<PrintWindow>();
            // Panels
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
            return services;
        }

        public static ServiceCollection AddViewModels(this ServiceCollection services)
        { 
            // Top menu
            services.AddSingleton<TopMenuViewModel>();
            // Windows
            services.AddSingleton<WelcomeWindowViewModel>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<PrintWindowViewModel>();
            // Panels
            services.AddSingleton<CardInspectorViewModel>();
            services.AddSingleton<CardSearcherViewModel>();
            services.AddSingleton<PrecoListViewModel>();
            services.AddSingleton<DeckListViewModel>();
            services.AddSingleton<SetListViewModel>();
            services.AddSingleton<CollecEstimationViewModel>();
            services.AddSingleton<TxtInputViewModel>();
            // Documents
            services.AddTransient<DocumentViewModel>();
            return services;
        }

    }

}
