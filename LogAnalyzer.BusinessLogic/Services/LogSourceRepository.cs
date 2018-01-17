using FileLogSource;
using LogAnalyzer.API.LogSource;
using LogAnalyzer.BusinessLogic.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.BusinessLogic.Services
{
    public class LogSourceRepository : ILogSourceRepository
    {
        private List<ILogSourceProvider> logSourceProviders;

        public LogSourceRepository()
        {
            logSourceProviders = new List<ILogSourceProvider>
            {
                new FileLogSourceProvider()
            };
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
