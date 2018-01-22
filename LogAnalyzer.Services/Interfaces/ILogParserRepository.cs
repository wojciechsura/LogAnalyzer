using LogAnalyzer.API.LogParser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Services.Interfaces
{
    public interface ILogParserRepository
    {
        IEnumerable<ILogParserProvider> LogParserProviders { get; }
    }
}
