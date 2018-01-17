using LogAnalyzer.BusinessLogic.Services;
using LogAnalyzer.BusinessLogic.Services.Interfaces;
using LogAnalyzer.BusinessLogic.ViewModels;
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
        public static void Configure(IUnityContainer container)
        {
            container.RegisterType<MainWindowViewModel>();

            container.RegisterType<ILogSourceRepository, LogSourceRepository>();
        }
    }
}
