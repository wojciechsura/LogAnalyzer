using Autofac;
using LogAnalyzer.Export.Services;
using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Export.Dependencies
{
    public static class Configuration
    {
        private static bool isConfigured = false;

        public static void Configure(ContainerBuilder builder)
        {
            if (isConfigured)
                return;

            builder.RegisterType<ExportService>().As<IExportService>().SingleInstance();

            isConfigured = true;
        }
    }
}
