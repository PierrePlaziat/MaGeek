using Microsoft.Extensions.DependencyInjection;
using MageekCore;
using MageekFrontWpf.UI.ViewModels.AppWindows;
using MageekFrontWpf.UI.ViewModels.AppPanels;
using MageekFrontWpf.UI.Views.AppWindows;
using MageekFrontWpf.UI.Views.AppPanels;
using MageekFrontWpf.UI.ViewModels;
using MageekCore.Data.Collection;
using MageekCore.Data.Mtg;
using MageekFrontWpf.UI.Views;
using MageekFrontWpf.UI.ViewModels.Windows;
using MageekFrontWpf.Framework.Services;
using MageekCore.Data;
using MageekFrontWpf.Framework.BaseMvvm;

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
            Folders.InitializeClientFolders();
            services.AddSingleton<CollectionDbManager>();
            services.AddSingleton<MtgDbManager>();
            services.AddSingleton<MageekService>();
            return services;
        }
        
        //public static ServiceCollection AddMageekClient(this ServiceCollection services)
        //{
        //}
        
        //public static ServiceCollection AddMageekServer(this ServiceCollection services)
        //{
        //}
        
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
            services.AddTransient<IDocument,DeckDocument>();

            // ViewModels //

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
            // Top menu
            services.AddSingleton<TopMenuViewModel>();

            return services;
        }

    }

}
