﻿using System;
using System.Windows;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.Framework.Services;
using MageekFrontWpf.UI.Views.AppWindows;
using MageekFrontWpf.AppValues;
using MageekCore.Data;
using MageekCore.Tools;

namespace MageekFrontWpf
{

    public partial class App : Application
    {

        private IServiceProvider serviceProvider;

        public App()
        {
            ServiceCollection services = new ServiceCollection();
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
            serviceProvider.GetService<WindowsService>().Initialize();
            WelcomeWindow welcome = serviceProvider.GetService<WelcomeWindow>();
            welcome.Show();
        }

        public static void Restart()
        {
            Logger.Log("RESTART");
            Process.Start(Process.GetCurrentProcess().MainModule.FileName);
            Current.Shutdown();
        }

    }

}
