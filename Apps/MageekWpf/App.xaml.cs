using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using MageekFrontWpf.Framework.Services;
using MageekFrontWpf.Framework.AppValues;
using MageekCore.Service;
using PlaziatTools;
using System.Threading.Tasks;
using System;

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
            DoStartup().ConfigureAwait(false);
        }
        private async Task DoStartup()
        {

            Logger.Log("Reaching server...");
            var connected = await ServiceHelper.GetService<IMageekService>().Client_Connect("https://172.26.0.1:8089/");
            Logger.Log("Connected : " + connected.ToString());
            if (connected == MageekCore.Data.MageekConnectReturn.Success) UseDistantServer();
            else UseLocalDb();
            ServiceHelper.GetService<WindowsService>().Startup();
        }

        private void UseDistantServer()
        {
            throw new NotImplementedException();
        }

        private void UseLocalDb()
        {
            throw new NotImplementedException();
        }
    }

}
