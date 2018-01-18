using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.API.LogParser
{
    public interface ILogParserProvider
    {
        ILogParserEditorViewModel CreateEditorViewModel();
        string UniqueName { get; }
    }
}
