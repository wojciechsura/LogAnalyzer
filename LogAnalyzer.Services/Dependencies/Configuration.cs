using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

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

            container.RegisterType<IWinApiService, WinApiService>();
            container.RegisterType<IConfigurationService, ConfigurationService>();
            container.RegisterType<IPathProviderService, PathProviderService>();

            isConfigured = true;
        }
    }
}
