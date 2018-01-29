using LogAnalyzer.API.LogParser;
using RegexLogParser.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegexLogParser
{
    public class RegexLogParserConfiguration : ILogParserConfiguration
    {
        public string Regex { get; set; }
        public List<BaseGroupDefinition> GroupDefinitions { get; set; }
    }
}
