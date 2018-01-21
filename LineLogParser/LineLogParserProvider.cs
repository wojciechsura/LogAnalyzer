using LineLogParser.Editor;
using LogAnalyzer.API.LogParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace LineLogParser
{
    public class LineLogParserProvider : ILogParserProvider
    {
        private readonly string UNIQUE_NAME = "LogParser.Line";

        public ILogParserEditorViewModel CreateEditorViewModel()
        {
            return LogAnalyzer.Dependencies.Container.Instance.Resolve<LineLogParserEditorViewModel>();
        }

        public string UniqueName => UNIQUE_NAME;
    }
}
