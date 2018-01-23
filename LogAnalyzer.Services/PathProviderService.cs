using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Services
{
    class PathProviderService : IPathProviderService
    {
        private static readonly string CONFIGURATION_FILE = "config.xml";

        public string GetConfigurationFilePath()
        {
            return Path.Combine(GetUserPath(), CONFIGURATION_FILE);
        }

        public string GetUserPath()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Spooksoft", "LogAnalyzer");
            if (!path.EndsWith("\\"))
                path += "\\";

            return path;
        }
    }
}
