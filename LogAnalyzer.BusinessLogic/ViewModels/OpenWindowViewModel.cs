using LogAnalyzer.API.LogSource;
using LogAnalyzer.BusinessLogic.Infrastructure;
using LogAnalyzer.BusinessLogic.Models;
using LogAnalyzer.BusinessLogic.Services.Interfaces;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LogAnalyzer.BusinessLogic.ViewModels
{
    public class OpenWindowViewModel : INotifyPropertyChanged
    {
        // Private fields -----------------------------------------------------

        private readonly IOpenWindowAccess access;
        private readonly ModalDialogResult<OpenResult> result;
        private readonly ILogSourceRepository logSourceRepository;

        private ObservableCollection<ILogSourceEditorViewModel> logSourceViewModels;
        private ILogSourceEditorViewModel selectedLogSource;

        // Private methods ----------------------------------------------------

        private void OnSelectedViewModelChanged()
        {
            
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Public methods -----------------------------------------------------

        public OpenWindowViewModel(IOpenWindowAccess access, ModalDialogResult<OpenResult> result, ILogSourceRepository logSourceRepository)
        {
            this.access = access;
            this.result = result;
            this.logSourceRepository = logSourceRepository;

            logSourceViewModels = logSourceRepository.CreateLogSourceViewModels();
            selectedLogSource = logSourceViewModels.FirstOrDefault();
        }

        // Public properties --------------------------------------------------

        public ObservableCollection<ILogSourceEditorViewModel> LogSources => logSourceViewModels;

        public ILogSourceEditorViewModel SelectedLogSource
        {
            get
            {
                return selectedLogSource;
            }
            set
            {
                selectedLogSource = value;
                OnSelectedViewModelChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
