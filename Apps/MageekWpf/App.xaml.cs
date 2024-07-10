using System;
using System.IO;
using System.Windows;
using PlaziatWpf.Mvvm;
using PlaziatWpf.Services;
using MageekDesktop.Framework;
using MageekDesktop.UI.Views.AppWindows;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using PlaziatTools;

namespace MageekDesktop
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
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                    Assembly.GetExecutingAssembly().GetName().Name
                ),
                ServiceHelper.GetService<MainWindow>().DockingManager,
                AppElements.LoadWindows(),
                AppElements.LoadTools()
            );
            ServiceHelper.GetService<WindowsService>().OpenWindow(AppWindowEnum.Welcome.ToString());
        }

        public static void Launch(string user)
        {
            ServiceHelper.GetService<SessionService>().User = user;
            ServiceHelper.GetService<WindowsService>().CloseWindow(AppWindowEnum.Welcome.ToString());
            ServiceHelper.GetService<WindowsService>().OpenWindow(AppWindowEnum.Main.ToString());
            ServiceHelper.GetService<WindowsService>().LoadLayout("Default");
            WeakReferenceMessenger.Default.Send(new LaunchAppMessage(""));
        }

    }

}
