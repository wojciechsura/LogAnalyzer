using LogAnalyzer.Services;
using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;

namespace LogAnalyzer.Dependencies
{
    public static class Configuration
    {
        private static bool isConfigured = false;

        public static void Configure(IUnityContainer container)
        {
            if (isConfigured)
                return;

            LogAnalyzer.BusinessLogic.Dependencies.Configuration.Configure(container);

            container.RegisterType<IDialogService, DialogService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMessagingService, MessagingService>(new ContainerControlledLifetimeManager());

            isConfigured = true;
        }
    }
}
