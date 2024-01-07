using System;
using System.Diagnostics;
using System.Windows;
using MaGeek.UI;
using MaGeek.UI.Menus;
using MageekFrontWpf;
using MageekFrontWpf.Framework;
using MageekFrontWpf.ViewModels;
using MageekService.Data.Collection;
using Microsoft.Extensions.DependencyInjection;

namespace MaGeek
{

    public partial class App : Application
    {

        private ServiceProvider serviceProvider;

        public static AppEvents Events { get; private set; } 
        public static AppState State { get; private set; }
        public static UserSettings Config { get; private set; }

        public App()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<ListImporter>();
            
            // Views
            services.AddSingleton<WelcomeWindow>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<ImporterUi>();
            services.AddTransient<TxtImporter>();
            // ViewModels
            services.AddSingleton<WelcomeViewModel>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<ImporterUiViewModel>();
            services.AddSingleton<ImporterUiViewModel>();
            services.AddTransient<TxtImporterViewModel>();

            services.AddSingleton<WinManager>();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            Events = new();
            Config = new();
            State = new();

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
