using LogAnalyzer.API.LogSource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Services.Interfaces
{
    public interface ILogSourceRepository
    {
        IEnumerable<ILogSourceProvider> LogSourceProviders { get; }
    }
}
