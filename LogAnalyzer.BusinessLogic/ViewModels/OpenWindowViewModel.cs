using LogAnalyzer.API.LogSource;
using LogAnalyzer.BusinessLogic.Models.OpenWindow;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using LogAnalyzer.Configuration;
using LogAnalyzer.Services.Common;
using LogAnalyzer.Services.Interfaces;
using LogAnalyzer.Services.Models;
using LogAnalyzer.Services.Models.DialogResults;
using LogAnalyzer.Services.Models.DialolgResults;
using LogAnalyzer.Wpf.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

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
        private readonly IDialogService dialogService;

        private ObservableCollection<ILogSourceEditorViewModel> logSourceViewModels;
        private ILogSourceEditorViewModel selectedLogSource;

        private ObservableCollection<LogParserProfileInfo> logParserProfiles;
        private LogParserProfileInfo selectedParserProfile;

        // Private methods ----------------------------------------------------

        private void BuildLogParserProfileInfos()
        {
            logParserProfiles.Clear();
            configurationService.Configuration.LogParserProfiles
                .Select(pp => new LogParserProfileInfo
                {
                    Name = pp.Name,
                    Guid = pp.Guid
                })
                .ToList()
                .ForEach(pp => logParserProfiles.Add(pp));
        }

        private void OnSelectedLogSourceChanged()
        {
            OnPropertyChanged(nameof(SelectedLogSource));
        }

        private void OnSelectedParserChanged()
        {
            OnPropertyChanged(nameof(selectedParserProfile));
        }

        private void DoEditParserProfile()
        {
            ModalDialogResult<LogParserProfileEditorResult> result = dialogService.EditLogParserProfile(selectedParserProfile.Guid);
            if (result.DialogResult)
            {
                BuildLogParserProfileInfos();
                SelectedLogParserProfile = logParserProfiles
                    .Where(p => p.Guid.Equals(result.Result.Guid))
                    .FirstOrDefault();
            }
        }

        private void DoNewParserProfile()
        {
            ModalDialogResult<LogParserProfileEditorResult> result = dialogService.NewLogParserProfile();
            if (result.DialogResult)
            {
                BuildLogParserProfileInfos();
                SelectedLogParserProfile = logParserProfiles
                    .Where(p => p.Guid.Equals(result.Result.Guid))
                    .FirstOrDefault();
            }
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Public methods -----------------------------------------------------

        public OpenWindowViewModel(IOpenWindowAccess access, 
            ILogSourceRepository logSourceRepository, 
            ILogParserRepository logParserRepository, 
            IConfigurationService configurationService,
            IDialogService dialogService)
        {
            this.access = access;
            this.logSourceRepository = logSourceRepository;
            this.logParserRepository = logParserRepository;
            this.configurationService = configurationService;
            this.dialogService = dialogService;

            NewParserProfileCommand = new SimpleCommand((obj) => DoNewParserProfile());
            EditParserProfileCommand = new SimpleCommand((obj) => DoEditParserProfile());

            result = new ModalDialogResult<OpenResult>();

            logSourceViewModels = logSourceRepository.CreateLogSourceViewModels();
            selectedLogSource = logSourceViewModels.FirstOrDefault();

            logParserProfiles = new ObservableCollection<LogParserProfileInfo>();
            BuildLogParserProfileInfos();
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

        public ObservableCollection<LogParserProfileInfo> LogParserProfiles => logParserProfiles;

        public LogParserProfileInfo SelectedLogParserProfile
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

        public ICommand NewParserProfileCommand { get; private set; }

        public ICommand EditParserProfileCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
