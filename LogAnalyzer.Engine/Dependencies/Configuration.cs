using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;

namespace LogAnalyzer.Engine.Dependencies
{
    public static class Configuration
    {
        private static bool isConfigured = false;

        public static void Configure(IUnityContainer container)
        {
            if (isConfigured)
                return;

            container.RegisterType<IEngineFactory, EngineFactory>(new ContainerControlledLifetimeManager());

            isConfigured = true;
        }
    }
}
