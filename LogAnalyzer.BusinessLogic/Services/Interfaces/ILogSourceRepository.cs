using LogAnalyzer.API.LogSource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.BusinessLogic.Services.Interfaces
{
    public interface ILogSourceRepository
    {
        ObservableCollection<ILogSourceEditorViewModel> CreateLogSourceViewModels();
    }
}
