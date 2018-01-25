using LogAnalyzer.API.LogSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.DialogResults
{
    public class OpenResult
    {
        public OpenResult(ILogSourceConfiguration logSourceConfiguration, ILogSourceProvider logSourceProvider, Guid parserProfileGuid)
        {
            LogSourceConfiguration = logSourceConfiguration;
            LogSourceProvider = logSourceProvider;
            ParserProfileGuid = parserProfileGuid;
        }

        public ILogSourceConfiguration LogSourceConfiguration { get; }
        public ILogSourceProvider LogSourceProvider { get; }
        public Guid ParserProfileGuid { get; }
    }
}
