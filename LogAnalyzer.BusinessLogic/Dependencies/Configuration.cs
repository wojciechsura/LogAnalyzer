using LogAnalyzer.BusinessLogic.Services;
using LogAnalyzer.BusinessLogic.ViewModels;
using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;

namespace LogAnalyzer.BusinessLogic.Dependencies
{
    public static class Configuration
    {
        private static bool isConfigured = false;

        public static void Configure(IUnityContainer container)
        {
            if (isConfigured)
                return;

            LogAnalyzer.Services.Dependencies.Configuration.Configure(container);
            LogAnalyzer.Mapper.Dependencies.Configuration.Configure(container);
            LogAnalyzer.Engine.Dependencies.Configuration.Configure(container);
            LogAnalyzer.TextParser.Dependencies.Configuration.Configure(container);
            LogAnalyzer.Export.Dependencies.Configuration.Configure(container);
            LogAnalyzer.Licensing.Dependencies.Configuration.Configure(container);

            container.RegisterType<MainWindowViewModel>();

            container.RegisterType<ILogSourceRepository, LogSourceRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<ILogParserRepository, LogParserRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<IScriptApiSampleRepository, ScriptApiSampleRepository>(new ContainerControlledLifetimeManager());

            isConfigured = true;
        }
    }
}
