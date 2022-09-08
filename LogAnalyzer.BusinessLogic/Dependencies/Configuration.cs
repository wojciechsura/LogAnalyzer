using Autofac;
using LogAnalyzer.BusinessLogic.Services;
using LogAnalyzer.BusinessLogic.ViewModels;
using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.BusinessLogic.Dependencies
{
    public static class Configuration
    {
        private static bool isConfigured = false;

        public static void Configure(ContainerBuilder builder)
        {
            if (isConfigured)
                return;

            LogAnalyzer.Services.Dependencies.Configuration.Configure(builder);
            LogAnalyzer.Mapper.Dependencies.Configuration.Configure(builder);
            LogAnalyzer.Engine.Dependencies.Configuration.Configure(builder);
            LogAnalyzer.TextParser.Dependencies.Configuration.Configure(builder);
            LogAnalyzer.Export.Dependencies.Configuration.Configure(builder);

            builder.RegisterType<LogSourceRepository>().As<ILogSourceRepository>().SingleInstance();
            builder.RegisterType<LogParserRepository>().As<ILogParserRepository>().SingleInstance();
            builder.RegisterType<ScriptApiSampleRepository>().As<IScriptApiSampleRepository>().SingleInstance();

            isConfigured = true;
        }
    }
}
