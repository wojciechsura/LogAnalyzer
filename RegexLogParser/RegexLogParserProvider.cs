using LogAnalyzer.API.LogParser;
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
            throw new NotImplementedException();
        }

        public ILogParserConfiguration DeserializeConfiguration(string serializedProfile)
        {
            throw new NotImplementedException();
        }

        public string SerializeConfiguration(ILogParserConfiguration configuration)
        {
            throw new NotImplementedException();
        }

        public string UniqueName => Consts.UNIQUE_NAME;
    }
}
