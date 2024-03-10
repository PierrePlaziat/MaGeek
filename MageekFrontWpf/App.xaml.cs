using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using MageekFrontWpf.Framework.Services;
using MageekFrontWpf.Framework.AppValues;
using MageekCore.Data;

namespace MageekFrontWpf
{

    public partial class App : Application
    {

        public App()
        {
            //Folders.InitializeClientFolders();
            ServiceCollection services = new ServiceCollection();
            services.AddFrameworkServices();
            services.AddAppServices();
            services.AddAppElements();
            ServiceHelper.Initialize(services.BuildServiceProvider());
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            ServiceHelper.GetService<WindowsService>().Startup();
        }

    }

}
