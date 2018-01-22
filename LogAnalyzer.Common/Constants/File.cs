using LogAnalyzer.Models.Services.WinApiService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Common.Constants
{
    public static class File
    {
        public static readonly List<FilterDefinition> LogFilterDefinitions = new List<FilterDefinition>
        {
            new FilterDefinition("*.log;*.txt", "All log files (*.log, *.txt)"),
            new FilterDefinition("*.log", "Log files (*.log)"),
            new FilterDefinition("*.txt", "Text files (*.txt)"),
            new FilterDefinition("*.*", "All files (*.*)")
        };
    }
}
