using System.Windows;
using PlaziatWpf.Mvvm;
using PlaziatWpf.Services;
using MageekDesktop.Framework;
using MageekDesktop.UI.Views.AppWindows;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace MageekDesktop
{

    public partial class App : Application
    {

        public App()
        {
            // Initialize services
            ServiceCollection services = new ServiceCollection();
            services.AddPlaziatFramework();
            services.AddBusiness();
            services.AddViewModels();
            services.AddViews();
            ServiceHelper.Initialize(services.BuildServiceProvider());
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            // Launch welcome window
            ServiceHelper.GetService<WindowsService>().Init(
                ServiceHelper.GetService<MainWindow>().DockingManager,
                AppElements.LoadWindows(),
                AppElements.LoadTools()
            );
            ServiceHelper.GetService<WindowsService>().OpenWindow(AppWindowEnum.Welcome.ToString());
        }

        public static void OnConnected(string userName)
        {
            // Launch main window
            ServiceHelper.GetService<SessionBag>().UserName = userName;
            ServiceHelper.GetService<WindowsService>().CloseWindow(AppWindowEnum.Welcome.ToString());
            ServiceHelper.GetService<WindowsService>().OpenWindow(AppWindowEnum.Main.ToString());
            ServiceHelper.GetService<WindowsService>().LoadLayout("Cached");
            WeakReferenceMessenger.Default.Send(new LaunchAppMessage(""));
        }

    }

}
