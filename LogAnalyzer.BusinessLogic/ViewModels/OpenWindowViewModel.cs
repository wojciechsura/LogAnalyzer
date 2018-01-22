using LogAnalyzer.API.LogSource;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using LogAnalyzer.Configuration;
using LogAnalyzer.Services.Interfaces;
using LogAnalyzer.Services.Models;
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
        private readonly ILogParserRepository logParserRepository;
        private readonly IConfigurationService configurationService;

        private ObservableCollection<ILogSourceEditorViewModel> logSourceViewModels;
        private ILogSourceEditorViewModel selectedLogSource;

        private ObservableCollection<LogParserProfile> logParserProfiles;
        private LogParserProfile selectedParserProfile;

        // Private methods ----------------------------------------------------

        private void OnSelectedLogSourceChanged()
        {
            OnPropertyChanged(nameof(SelectedLogSource));
        }

        private void OnSelectedParserChanged()
        {
            OnPropertyChanged(nameof(selectedParserProfile));
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Public methods -----------------------------------------------------

        public OpenWindowViewModel(IOpenWindowAccess access, ILogSourceRepository logSourceRepository, ILogParserRepository logParserRepository, IConfigurationService configurationService)
        {
            this.access = access;
            this.logSourceRepository = logSourceRepository;
            this.logParserRepository = logParserRepository;
            this.configurationService = configurationService;

            result = new ModalDialogResult<OpenResult>();

            logSourceViewModels = logSourceRepository.CreateLogSourceViewModels();
            selectedLogSource = logSourceViewModels.FirstOrDefault();

            logParserProfiles = new ObservableCollection<LogParserProfile>();
            configurationService.Configuration.LogParserProfiles
                .ForEach(pp => logParserProfiles.Add(pp));
            SelectedLogParserProfile = logParserProfiles.FirstOrDefault();
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
                OnSelectedLogSourceChanged();
            }
        }

        public ObservableCollection<LogParserProfile> LogParserProfiles => logParserProfiles;

        public LogParserProfile SelectedLogParserProfile
        {
            get
            {
                return selectedParserProfile;
            }
            set
            {
                selectedParserProfile = value;
                OnSelectedParserChanged();
            }
        }

        public ModalDialogResult<OpenResult> Result => result;

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
