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

        string DisplayName { get; }
        string EditorResource { get; }
        bool Validate();

        ILogParserProvider Provider { get; }
    }
}
