using Microsoft.Extensions.DependencyInjection;
using System;

namespace MageekFrontWpf.Framework.BaseMvvm
{

    public static class ServiceHelper
    {
        public static IServiceProvider Services { get; private set; }
        public static void Initialize(IServiceProvider services) => Services = services;
        public static T GetService<T>() => Services.GetService<T>();
    }

}
