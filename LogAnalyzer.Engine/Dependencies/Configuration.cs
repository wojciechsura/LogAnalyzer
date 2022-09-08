using Autofac;
using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Dependencies
{
    public static class Configuration
    {
        private static bool isConfigured = false;

        public static void Configure(ContainerBuilder builder)
        {
            if (isConfigured)
                return;

            builder.RegisterType<EngineFactory>().As<IEngineFactory>().SingleInstance();

            isConfigured = true;
        }
    }
}
