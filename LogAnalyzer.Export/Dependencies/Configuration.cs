using LogAnalyzer.Export.Services;
using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;
using Unity.Resolution;

namespace LogAnalyzer.Export.Dependencies
{
    public static class Configuration
    {
        private static bool isConfigured = false;

        public static void Configure(IUnityContainer container)
        {
            if (isConfigured)
                return;

            container.RegisterType<IExportService, ExportService>(new ContainerControlledLifetimeManager());

            isConfigured = true;
        }
    }
}
