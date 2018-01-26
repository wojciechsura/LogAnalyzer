using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;

namespace LogAnalyzer.Services.Dependencies
{
    public static class Configuration
    {
        private static bool isConfigured = false;

        public static void Configure(IUnityContainer container)
        {
            if (isConfigured)
                return;

            // Configure

            container.RegisterType<IWinApiService, WinApiService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IConfigurationService, ConfigurationService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPathProviderService, PathProviderService>(new ContainerControlledLifetimeManager());

            isConfigured = true;
        }
    }
}
