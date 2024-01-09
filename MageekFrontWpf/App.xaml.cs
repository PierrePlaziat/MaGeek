using System;
using System.Diagnostics;
using System.Windows;
using MaGeek.UI;
using MaGeek.UI.Menus;
using MaGeek.UI.Windows.Importers;
using MageekFrontWpf;
using MageekFrontWpf.Framework;
using MageekFrontWpf.Framework.Services;
using MageekFrontWpf.ViewModels;
using MageekService;
using Microsoft.Extensions.DependencyInjection;

namespace MaGeek
{

    public partial class App : Application
    {

        private ServiceProvider serviceProvider;

        public App()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<CollectionImporter>();
            // Framework
            services.AddSingleton<DialogService>();
            services.AddSingleton<SettingService>();
            // App
            services.AddSingleton<AppEvents>();
            services.AddSingleton<AppState>();
            // Views
            services.AddSingleton<WelcomeWindow>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<ImporterUi>();
            services.AddSingleton<TxtImporter>();
            services.AddSingleton<PrecoImporter>();
            services.AddSingleton<DeckStats>();
            services.AddSingleton<CardInspector>();
            services.AddSingleton<CardSearcher>();
            services.AddSingleton<DeckContent>();
            services.AddSingleton<DeckList>();
            services.AddSingleton<DeckTable>();
            // ViewModels
            services.AddSingleton<WelcomeViewModel>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<ImporterUiViewModel>();
            services.AddSingleton<TxtImporterViewModel>();
            services.AddSingleton<PrecoImporterViewModel>();
            services.AddSingleton<DeckStatsViewModel>();
            services.AddSingleton<CardInspectorViewModel>();
            services.AddSingleton<CardSearcherViewModel>();
            services.AddSingleton<DeckContentViewModel>();
            services.AddSingleton<DeckListViewModel>();
            services.AddSingleton<DeckTableViewModel>();
            // WindowsManager
            services.AddSingleton<WindowsManager>();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var startWindow = serviceProvider.GetService<WelcomeWindow>();
            startWindow.Show();
        }

        public static void Restart()
        {
            Process.Start(Process.GetCurrentProcess().MainModule.FileName);
            Current.Shutdown();
        }

        public static void Quit()
        {
            Environment.Exit(-1);
        }

    }

}
