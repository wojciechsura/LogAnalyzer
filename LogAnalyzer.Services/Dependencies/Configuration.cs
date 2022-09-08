using Autofac;
using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Services.Dependencies
{
    public static class Configuration
    {
        private static bool isConfigured = false;

        public static void Configure(ContainerBuilder builder)
        {
            if (isConfigured)
                return;

            // Configure

            builder.RegisterType<WinApiService>().As<IWinApiService>().SingleInstance();
            builder.RegisterType<ConfigurationService>().As<IConfigurationService>().SingleInstance();
            builder.RegisterType<PathProviderService>().As<IPathProviderService>().SingleInstance();
            builder.RegisterType<EventBusService>().As<IEventBusService>().SingleInstance();

            isConfigured = true;
        }
    }
}
