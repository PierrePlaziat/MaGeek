using System;
using System.Windows;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using WPFNotification.Services;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.Framework.Services;
using MageekFrontWpf.UI.Views.AppWindows;
using MageekFrontWpf.AppValues;

namespace MageekFrontWpf
{

    public partial class App : Application
    {

        private IServiceProvider serviceProvider;

        public App()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<INotificationDialogService, NotificationDialogService>();
            services.AddLogging();
            services.AddSingleton<WindowsService>();
            services.AddSingleton<DialogService>();
            services.AddSingleton<SettingService>();
            services.AddMageek();
            serviceProvider = services.BuildServiceProvider();
            ServiceHelper.Initialize(serviceProvider);
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            AppSettings.SetDefaultSettings(serviceProvider.GetService<SettingService>());
            serviceProvider.GetService<WindowsService>().Init();
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
