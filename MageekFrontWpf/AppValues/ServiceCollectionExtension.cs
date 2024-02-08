﻿using Microsoft.Extensions.DependencyInjection;
using MageekServices;
using MageekFrontWpf.Framework.Services;
using MageekFrontWpf.UI.ViewModels.AppWindows;
using MageekFrontWpf.UI.ViewModels.AppPanels;
using MageekFrontWpf.UI.Views.AppWindows;
using MageekFrontWpf.UI.Views.AppPanels;
using MageekFrontWpf.UI.ViewModels;
using MageekServices.Data.Collection;
using MageekServices.Data.Mtg;
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
            services.AddSingleton<TxtInputWindow>();
            // Tools
            services.AddSingleton<CardInspector>();
            services.AddSingleton<CardSearcher>();
            services.AddSingleton<PrecoList>();
            services.AddSingleton<DeckList>();
            services.AddSingleton<SetList>();
            services.AddSingleton<CollecEstimation>();

            // ViewModels //////////////////////////////////////////////////////////////

            services.AddSingleton<TopMenuViewModel>();
            // Windows
            services.AddSingleton<WelcomeWindowViewModel>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<PrintViewModel>();
            services.AddSingleton<TxtInputViewModel>();
            // Tools
            services.AddSingleton<CardInspectorViewModel>();
            services.AddSingleton<CardSearcherViewModel>();
            services.AddSingleton<PrecoListViewModel>();
            services.AddSingleton<DeckListViewModel>();
            services.AddSingleton<SetListViewModel>();
            services.AddSingleton<CollecEstimationViewModel>();

            // Opened decks ////////////////////////////////////////////////////////////
            
            services.AddTransient<DeckDocument>();
            services.AddTransient<DeckDocumentViewModel>();
            //services.AddTransient<DeckContent>();
            //services.AddTransient<DeckContentViewModel>();
            //services.AddTransient<DeckTable>();
            //services.AddTransient<DeckStats>();
            //services.AddTransient<DeckTableViewModel>();
            //services.AddTransient<DeckStatsViewModel>();

            ///////////////////////////////////////////////////////////////////////////

            return services;
        }
    }
}
