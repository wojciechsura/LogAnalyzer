using Autofac;
using DatabaseLogParser.Common;
using DatabaseLogParser.Editor;
using LogAnalyzer.API.LogParser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLogParser
{
    public class DatabaseLogParserProvider : ILogParserProvider
    {
        public ILogParserEditorViewModel CreateEditorViewModel()
        {
            return LogAnalyzer.Dependencies.Container.Instance.Resolve<DatabaseLogParserEditorViewModel>(new NamedParameter("parentProvider", this));
        }

        public ILogParser CreateParser(ILogParserConfiguration configuration)
        {
            return new DatabaseParser((DatabaseLogParserConfiguration)configuration);
        }

        public ILogParserConfiguration DeserializeConfiguration(string serializedProfile)
        {
            var configuration = JsonConvert.DeserializeObject<DatabaseLogParserConfiguration>(serializedProfile, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            return configuration;
        }

        public string SerializeConfiguration(ILogParserConfiguration configuration)
        {
            var regexConfiguration = (DatabaseLogParserConfiguration)configuration;

            string result = JsonConvert.SerializeObject(regexConfiguration, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            return result;
        }

        public string UniqueName => Consts.UNIQUE_NAME;
    }
}
