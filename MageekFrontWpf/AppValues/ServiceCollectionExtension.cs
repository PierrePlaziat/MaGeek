using Microsoft.Extensions.DependencyInjection;
using MageekCore;
using MageekFrontWpf.Framework.Services;
using MageekFrontWpf.UI.ViewModels.AppWindows;
using MageekFrontWpf.UI.ViewModels.AppPanels;
using MageekFrontWpf.UI.Views.AppWindows;
using MageekFrontWpf.UI.Views.AppPanels;
using MageekFrontWpf.UI.ViewModels;
using MageekCore.Data.Collection;
using MageekCore.Data.Mtg;
using MageekFrontWpf.UI.Views;

namespace MageekFrontWpf.AppValues
{
    public static class ServiceCollectionExtensions
    {

        public static ServiceCollection AddMageek(this ServiceCollection services)
        {

            // Metier //////////////////////////////////////////////////////////////////

            services.AddSingleton<CollectionDbManager>();
            services.AddSingleton<MtgDbManager>();
            services.AddSingleton<MageekService>();

            // Views ///////////////////////////////////////////////////////////////////

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
            services.AddTransient<DeckDocument>();

            // ViewModels //////////////////////////////////////////////////////////////

            services.AddSingleton<TopMenuViewModel>();
            // Windows
            services.AddSingleton<WelcomeWindowViewModel>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<PrintViewModel>();
            // Tools
            services.AddSingleton<CardInspectorViewModel>();
            services.AddSingleton<CardSearcherViewModel>();
            services.AddSingleton<PrecoListViewModel>();
            services.AddSingleton<DeckListViewModel>();
            services.AddSingleton<SetListViewModel>();
            services.AddSingleton<CollecEstimationViewModel>();
            services.AddSingleton<TxtInputViewModel>();
            // Documents
            services.AddTransient<DeckDocumentViewModel>();

            ////////////////////////////////////////////////////////////////////////////

            return services;

        }

    }

}
