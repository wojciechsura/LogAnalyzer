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
        public OpenResult(ILogSourceConfiguration logSourceConfiguration, string logSourceProviderName, Guid parserProfileGuid)
        {
            LogSourceConfiguration = logSourceConfiguration;
            LogSourceProviderName = logSourceProviderName;
            ParserProfileGuid = parserProfileGuid;
        }

        public ILogSourceConfiguration LogSourceConfiguration { get; }
        public string LogSourceProviderName { get; }
        public Guid ParserProfileGuid { get; }
    }
}
