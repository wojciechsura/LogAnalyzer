using LogAnalyzer.BusinessLogic.Services;
using LogAnalyzer.BusinessLogic.ViewModels;
using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

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

            container.RegisterType<MainWindowViewModel>();

            container.RegisterType<ILogSourceRepository, LogSourceRepository>();
            container.RegisterType<ILogParserRepository, LogParserRepository>();

            isConfigured = true;
        }
    }
}
