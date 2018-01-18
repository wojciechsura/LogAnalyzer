using FileLogSource;
using LogAnalyzer.API.LogSource;
using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.BusinessLogic.Services
{
    class LogSourceRepository : ILogSourceRepository
    {
        private List<ILogSourceProvider> logSourceProviders;

        public LogSourceRepository()
        {
            logSourceProviders = new List<ILogSourceProvider>
            {
                new FileLogSourceProvider()
            };

            // Verifying uniqueness of names
            if (logSourceProviders.Any(p => logSourceProviders.Any(q => q.UniqueName == p.UniqueName && p != q)))
                throw new InvalidOperationException("Not all log source providers names are unique!");
        }

        public ObservableCollection<ILogSourceEditorViewModel> CreateLogSourceViewModels()
        {
            var result = new ObservableCollection<ILogSourceEditorViewModel>();

            logSourceProviders.Select(lsp => lsp.CreateEditorViewModel())
                .ToList()
                .ForEach(vm => result.Add(vm));

            return result;
        }
    }
}
