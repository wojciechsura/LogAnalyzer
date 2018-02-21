using LogAnalyzer.Models.Services.WinApiService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Services.Interfaces
{
    public interface IWinApiService
    {
        string OpenFile(IEnumerable<FilterDefinition> filter);
        string SaveFile(IEnumerable<FilterDefinition> filter);
        List<string> OpenFiles(List<FilterDefinition> logFilterDefinitions);
        void StartProcess(string path);
    }
}
