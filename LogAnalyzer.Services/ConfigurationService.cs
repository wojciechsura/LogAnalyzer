using LogAnalyzer.Configuration;
using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Services
{
    class ConfigurationService : IConfigurationService
    {
        private ConfigurationRoot configuration;

        public ConfigurationService()
        {
            configuration = new ConfigurationRoot();
        }

        public ConfigurationRoot Configuration => configuration;
    }
}
