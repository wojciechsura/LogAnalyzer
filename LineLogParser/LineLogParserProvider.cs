using Autofac;
using LineLogParser.Editor;
using LogAnalyzer.API.LogParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineLogParser
{
    public class LineLogParserProvider : ILogParserProvider
    {
        public ILogParserEditorViewModel CreateEditorViewModel()
        {
            return LogAnalyzer.Dependencies.Container.Instance.Resolve<LineLogParserEditorViewModel>(new NamedParameter("provider", this));
        }

        public ILogParserConfiguration DeserializeConfiguration(string serializedProfile)
        {
            return new LineLogParserConfiguration();
        }

        public string SerializeConfiguration(ILogParserConfiguration configuration)
        {
            return String.Empty;
        }

        public ILogParser CreateParser(ILogParserConfiguration configuration)
        {
            return new LineLogParser();
        }

        public string UniqueName => Common.Consts.UNIQUE_NAME;
    }
}
