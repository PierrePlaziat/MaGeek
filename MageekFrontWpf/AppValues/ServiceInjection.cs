using Microsoft.Extensions.DependencyInjection;
using MageekCore;
using MageekCore.Data;
using MageekCore.Data.Mtg;
using MageekCore.Data.Collection;
using MageekFrontWpf.UI.ViewModels.AppWindows;
using MageekFrontWpf.UI.ViewModels.AppPanels;
using MageekFrontWpf.UI.Views.AppWindows;
using MageekFrontWpf.UI.Views.AppPanels;
using MageekFrontWpf.UI.ViewModels;
using MageekFrontWpf.UI.Views;
using MageekFrontWpf.UI.ViewModels.Windows;
using MageekFrontWpf.Framework.Services;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.MageekTools.DeckTools;

namespace MageekFrontWpf.Framework.AppValues
{
    public static class ServiceCollectionExtensions
    {

        public static ServiceCollection AddFrameworkServices(this ServiceCollection services)
        {
            services.AddSingleton<WindowsService>();
            services.AddSingleton<DialogService>();
            services.AddSingleton<SettingService>();
            return services;
        }
        
        public static ServiceCollection AddAppServices(this ServiceCollection services)
        {
            //services.AddSingleton<CollectionDbManager>();
            //services.AddSingleton<MtgDbManager>();
            services.AddSingleton<MageekService>();
            services.AddSingleton<DeckManipulator>();
            return services;
        }
        
        public static ServiceCollection AddAppElements(this ServiceCollection services)
        {
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
            services.AddTransient<IDocument,Document>();
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
