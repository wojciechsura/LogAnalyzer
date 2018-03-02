using LogAnalyzer.API.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.API.LogParser
{
    public interface ILogParserEditorViewModel
    {
        void SetConfiguration(ILogParserConfiguration configuration);
        ILogParserConfiguration GetConfiguration();
        void SetSampleLines(List<string> sampleLines);

        string DisplayName { get; }
        string EditorResource { get; }
        ValidationResult Validate();

        ILogParserProvider Provider { get; }
    }
}
