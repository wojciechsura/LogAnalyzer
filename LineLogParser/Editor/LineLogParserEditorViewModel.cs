using LogAnalyzer.API.LogParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineLogParser.Editor
{
    public class LineLogParserEditorViewModel : ILogParserEditorViewModel
    {
        private readonly string DISPLAY_NAME = "Parse whole line as message";
        private readonly string EDITOR_RESOURCE = "LineLogEditorTemplate";

        public LineLogParserEditorViewModel(ILogParserProvider provider)
        {
            Provider = provider;
        }

        public void SetConfiguration(ILogParserConfiguration configuration)
        {
            
        }

        public ILogParserConfiguration GetConfiguration()
        {
            return new LineLogParserConfiguration();
        }

        public bool Validate()
        {
            return true;
        }

        public string DisplayName => DISPLAY_NAME;
        public string EditorResource => EDITOR_RESOURCE;
        public ILogParserProvider Provider { get; }
    }
}
