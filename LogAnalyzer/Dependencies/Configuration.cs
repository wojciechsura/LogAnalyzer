using Autofac;
using LogAnalyzer.Services;
using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Dependencies
{
    public static class Configuration
    {
        private static bool isConfigured = false;

        public static void Configure(ContainerBuilder builder)
        {
            if (isConfigured)
                return;

            LogAnalyzer.BusinessLogic.Dependencies.Configuration.Configure(builder);

            builder.RegisterType<DialogService>().As<IDialogService>().SingleInstance();
            builder.RegisterType<MessagingService>().As<IMessagingService>().SingleInstance();

            isConfigured = true;
        }
    }
}
