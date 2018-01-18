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
        public string UniqueName => throw new NotImplementedException();

        public ILogParserEditorViewModel CreateEditorViewModel()
        {
            return LogAnalyzer.Dependencies.Container.Instance.Resolve<LineLogParserEditorViewModel>();
        }
    }
}
