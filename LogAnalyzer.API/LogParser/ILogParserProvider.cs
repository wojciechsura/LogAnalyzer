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
        ILogParserConfiguration DeserializeConfiguration(string serializedProfile);
        string SerializeConfiguration(ILogParserConfiguration configuration);
        ILogParser CreateParser(ILogParserConfiguration configuration);

        string UniqueName { get; }
    }
}
