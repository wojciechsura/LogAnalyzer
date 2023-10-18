using LogAnalyzer.API.LogParser;
using DatabaseLogParser.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLogParser
{
    public class DatabaseLogParserConfiguration : ILogParserConfiguration
    {
        public List<BaseFieldDefinition> FieldDefinitions { get; set; }
    }
}
