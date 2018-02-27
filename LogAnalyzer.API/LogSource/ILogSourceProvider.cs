using LogAnalyzer.API.LogParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.API.LogSource
{
    public interface ILogSourceProvider
    {
        ILogSourceEditorViewModel CreateEditorViewModel();
        ILogSource CreateLogSource(ILogSourceConfiguration configuration, ILogParser logParser);
        string UniqueName { get; }
        ILogSourceConfiguration CreateFromLocalPaths(List<string> files);
        ILogSourceConfiguration CreateFromClipboard();
    }
}
