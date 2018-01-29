using LogAnalyzer.Configuration;
using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LogAnalyzer.Services
{
    class ConfigurationService : IConfigurationService
    {
        private AppConfiguration configuration;
        private readonly IPathProviderService pathProviderService;

        private void HandleConfigurationChanged(object sender, EventArgs e)
        {
            string configFilename = pathProviderService.GetConfigurationFilePath();
            Directory.CreateDirectory(Path.GetDirectoryName(configFilename));
            configuration.Save(configFilename);
        }

        public ConfigurationService(IPathProviderService pathProviderService)
        {
            configuration = new AppConfiguration();
            configuration.ConfigurationChanged += HandleConfigurationChanged;

            this.pathProviderService = pathProviderService;
            LoadConfiguration();            
        }

        public void LoadConfiguration()
        {
            try
            {
                configuration.Load(pathProviderService.GetConfigurationFilePath());
            }
            catch 
            {
                configuration.Defaults();
            }
        }

        public void SaveConfiguration()
        {
            configuration.Save(pathProviderService.GetConfigurationFilePath());
        }

        public AppConfiguration Configuration => configuration;
    }
}
