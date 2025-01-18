using Microsoft.Extensions.DependencyInjection;
using MageekDesktopClient.UI.ViewModels.AppWindows;
using MageekDesktopClient.UI.ViewModels.AppPanels;
using MageekDesktopClient.UI.Views.AppWindows;
using MageekDesktopClient.UI.Views.AppPanels;
using MageekDesktopClient.UI.ViewModels;
using MageekDesktopClient.UI.Views;
using MageekDesktopClient.UI.ViewModels.Windows;
using MageekDesktopClient.MageekTools.DeckTools;
using MageekClient.Services;
using MageekCore.Services;
using PlaziatWpf.Docking;
using MageekDesktopClient.DeckTools;

namespace MageekDesktopClient.Framework
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
            services.AddSingleton<Detector>();
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
            services.AddSingleton<DetectorViewModel>();
            // Documents
            services.AddTransient<DocumentViewModel>();
            return services;
        }

    }

}
