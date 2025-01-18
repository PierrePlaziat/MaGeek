using System.Windows;
using PlaziatWpf.Mvvm;
using PlaziatWpf.Services;
using MageekDesktopClient.Framework;
using MageekDesktopClient.UI.Views.AppWindows;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using PlaziatTools;
using System;
using MageekCore.Data.Collection.Entities;
using MageekCore.Services;
using System.Threading.Tasks;

namespace MageekDesktopClient
{

    public partial class App : Application
    {

        public App()
        {
            ServiceCollection services = new ServiceCollection()
                .AddPlaziatFramework()
                .AddBusiness()
                .AddViewModels()
                .AddViews();
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
            LoadLayout().ConfigureAwait(false);
        }

        public static async Task LoadLayout()
        {
                var docs = ServiceHelper.GetService<WindowsService>().LoadLayout("Cached");
                foreach (var docId in docs)
                {
                    try
                    {
                        if (docId.StartsWith("["))
                        {
                            // Wont work
                            WeakReferenceMessenger.Default.Send(
                                new PrintDeckMessage(docId)
                            );
                        }
                        else
                        {
                            Deck deck = await ServiceHelper.GetService<IMageekService>().Decks_Get(
                                ServiceHelper.GetService<SessionBag>().UserName,
                                docId
                            );
                            ServiceHelper.GetService<WindowsService>().OpenDocument(new DocumentArguments(deck: deck), true);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Log(e);
                    }
                }
                WeakReferenceMessenger.Default.Send(new LaunchAppMessage(""));
        }
    }

}
