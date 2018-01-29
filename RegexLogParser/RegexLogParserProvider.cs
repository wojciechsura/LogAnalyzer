using LogAnalyzer.API.LogParser;
using Newtonsoft.Json;
using RegexLogParser.Common;
using RegexLogParser.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegexLogParser
{
    public class RegexLogParserProvider : ILogParserProvider
    {
        public ILogParserEditorViewModel CreateEditorViewModel()
        {
            return new RegexLogParserEditorViewModel(this);
        }

        public ILogParser CreateParser(ILogParserConfiguration configuration)
        {
            return new RegexLogParser((RegexLogParserConfiguration)configuration);
        }

        public ILogParserConfiguration DeserializeConfiguration(string serializedProfile)
        {
            var configuration = JsonConvert.DeserializeObject<RegexLogParserConfiguration>(serializedProfile, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            return configuration;
        }

        public string SerializeConfiguration(ILogParserConfiguration configuration)
        {
            var regexConfiguration = (RegexLogParserConfiguration)configuration;

            string result = JsonConvert.SerializeObject(regexConfiguration, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            return result;
        }

        public string UniqueName => Consts.UNIQUE_NAME;
    }
}
