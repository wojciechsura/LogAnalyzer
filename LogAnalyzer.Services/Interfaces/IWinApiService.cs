using LogAnalyzer.Models.Services.WinApiService;
using LogAnalyzer.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Services.Interfaces
{
    public interface IWinApiService
    {
        String OpenFile(IEnumerable<FilterDefinition> filter);
    }
}
