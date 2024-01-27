using System;
using System.Diagnostics;
using System.Windows;
using MageekFrontWpf;
using MageekFrontWpf.App;
using MageekFrontWpf.Framework.BaseMvvm;
using Microsoft.Extensions.DependencyInjection;
using WPFNotification.Services;

namespace MaGeek
{

    public partial class App : Application
    {

        private IServiceProvider serviceProvider;

        public App()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddLogging();
            services.AddSingleton<INotificationDialogService, NotificationDialogService>();
            services.AddMageek();
            serviceProvider = services.BuildServiceProvider();
            ServiceHelper.Initialize(serviceProvider);
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            AppSettings.SetDefaultSettings(serviceProvider.GetService<SettingService>());
            serviceProvider.GetService<WindowsManager>().Init();
            WelcomeWindow welcome = serviceProvider.GetService<WelcomeWindow>();
            welcome.Show();
        }

        public static void Restart()
        { 
            Process.Start(Process.GetCurrentProcess().MainModule.FileName);
            Current.Shutdown();
        }

    }

}
