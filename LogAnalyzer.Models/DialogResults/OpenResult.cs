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
        public OpenResult(ILogSourceConfiguration logSourceConfiguration, string logSourceProviderName, Guid parserProfileGuid, Guid selectedProcessingProfileGuid)
        {
            LogSourceConfiguration = logSourceConfiguration;
            LogSourceProviderName = logSourceProviderName;
            ParserProfileGuid = parserProfileGuid;
            ProcessingProfileGuid = selectedProcessingProfileGuid;
        }

        public ILogSourceConfiguration LogSourceConfiguration { get; }
        public string LogSourceProviderName { get; }
        public Guid ParserProfileGuid { get; }
        public Guid ProcessingProfileGuid { get; }
    }
}
