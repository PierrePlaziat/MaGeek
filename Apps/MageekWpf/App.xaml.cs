using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using PlaziatWpf.Services;
using System;
using System.IO;
using MageekFrontWpf.UI.Views.AppWindows;
using CommunityToolkit.Mvvm.Messaging;
using PlaziatWpf.Mvvm;
using MageekFrontWpf.Framework;

namespace MageekFrontWpf
{

    public partial class App : Application
    {

        public App()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddFrameworkServices();
            services.AddMageek();
            ServiceHelper.Initialize(services.BuildServiceProvider());
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            ServiceHelper.GetService<WindowsService>().Init(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MaGeek"),
                ServiceHelper.GetService<MainWindow>().DockingManager,
                AppElements.LoadWindows(),
                AppElements.LoadTools()
            );
            ServiceHelper.GetService<WindowsService>().OpenWindow(AppWindowEnum.Welcome.ToString());
        }

        public static void Launch(string user)
        {
            //TODO stock user session 
            ServiceHelper.GetService<SessionService>().User = user;
            ServiceHelper.GetService<WindowsService>().CloseWindow("Welcome");
            ServiceHelper.GetService<WindowsService>().OpenWindow("Main");
            ServiceHelper.GetService<WindowsService>().LoadLayout("Default");
            WeakReferenceMessenger.Default.Send(new LaunchAppMessage(""));
        }

    }

}
