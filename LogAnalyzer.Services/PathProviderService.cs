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
        private static readonly string LICENSE_FILE = "loganalyzer.license";
        private static readonly string SCRIPTS_PATH = "Scripts";

        private void CreateIfNotExist(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public PathProviderService()
        {
            CreateIfNotExist(GetScriptsPath());
        }

        public string GetConfigurationFilePath()
        {
            return Path.Combine(GetUserPath(), CONFIGURATION_FILE);
        }

        public string GetLicenseFilePath()
        {
            return Path.Combine(GetUserPath(), LICENSE_FILE);
        }

        public string GetScriptsPath()
        {
            return Path.Combine(GetUserPath(), SCRIPTS_PATH);
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
