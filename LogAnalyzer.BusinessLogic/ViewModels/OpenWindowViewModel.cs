using LogAnalyzer.API.LogSource;
using LogAnalyzer.API.Types;
using LogAnalyzer.BusinessLogic.Models.Views.OpenWindow;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using LogAnalyzer.Configuration;
using LogAnalyzer.Models.DialogResults;
using LogAnalyzer.Models.Views.OpenWindow;
using LogAnalyzer.Services.Common;
using LogAnalyzer.Services.Interfaces;
using LogAnalyzer.Wpf.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly IMessagingService messagingService;

        private ObservableCollection<ILogSourceEditorViewModel> logSourceViewModels;
        private ILogSourceEditorViewModel selectedLogSource;

        private ObservableCollection<LogParserProfileInfo> logParserProfiles;
        private LogParserProfileInfo selectedParserProfile;
        private Condition parserProfileSelected;

        // Private methods ----------------------------------------------------

        private void BuildLogParserProfileInfos()
        {
            logParserProfiles.Clear();
            configurationService.Configuration.LogParserProfiles
                .Select(pp => new LogParserProfileInfo
                {
                    Name = pp.Name.Value,
                    Guid = pp.Guid.Value
                })
                .ToList()
                .ForEach(pp => logParserProfiles.Add(pp));
        }

        private List<string> GetSampleLines()
        {
            if (selectedLogSource.ProvidesSampleLines)
                return selectedLogSource.ProvideSampleLines();
            else
                return new List<string>();
        }

        private void OnSelectedLogSourceChanged()
        {
            OnPropertyChanged(nameof(SelectedLogSource));
        }

        private void OnSelectedParserChanged()
        {
            OnPropertyChanged(nameof(SelectedLogParserProfile));
        }

        private void DoEditParserProfile()
        {
            var sampleLines = GetSampleLines();

            ModalDialogResult<LogParserProfileEditorResult> result = dialogService.EditLogParserProfile(selectedParserProfile.Guid, sampleLines);
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
            var sampleLines = GetSampleLines();

            ModalDialogResult<LogParserProfileEditorResult> result = dialogService.NewLogParserProfile(sampleLines);
            if (result.DialogResult)
            {
                BuildLogParserProfileInfos();
                SelectedLogParserProfile = logParserProfiles
                    .Where(p => p.Guid.Equals(result.Result.Guid))
                    .FirstOrDefault();
            }
        }

        private void DoDeleteParserProfile()
        {
            if (messagingService.Ask("Are you sure you want to delete this profile?"))
            {
                var deletedProfile = configurationService.Configuration.LogParserProfiles.Where(p => p.Guid.Value == selectedParserProfile.Guid).Single();
                configurationService.Configuration.LogParserProfiles.Remove(deletedProfile);

                logParserProfiles.Remove(selectedParserProfile);
                SelectedLogParserProfile = logParserProfiles.FirstOrDefault();
            }
        }

        private void DoOk()
        {
            if (selectedLogSource == null)
            {
                messagingService.Warn("Choose the log source!");
                return;
            }

            ValidationResult validationResult = selectedLogSource.Validate();
            if (!validationResult.Valid)
            {
                messagingService.Warn(validationResult.Message);
                return;
            }

            if (selectedParserProfile == null)
            {
                messagingService.Warn("Choose parser profile first!");
                return;
            }

            result.DialogResult = true;
            result.Result = new OpenResult(selectedLogSource.BuildConfiguration(), selectedLogSource.Provider.UniqueName, selectedParserProfile.Guid);

            // Save configuration
            configurationService.Configuration.SuspendNotifications();
            try
            {
                configurationService.Configuration.Session.LastParserProfile.Value = selectedParserProfile.Guid;
            }
            finally
            { 
                configurationService.Configuration.ResumeNotifications();
            }

            access.Close(true);
        }

        private void DoCancel()
        {
            access.Close(false);
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Public methods -----------------------------------------------------

        public OpenWindowViewModel(IOpenWindowAccess access, 
            BaseOpenFilesModel model,
            ILogSourceRepository logSourceRepository, 
            ILogParserRepository logParserRepository, 
            IConfigurationService configurationService,
            IDialogService dialogService,
            IMessagingService messagingService)
        {
            this.access = access;
            this.logSourceRepository = logSourceRepository;
            this.logParserRepository = logParserRepository;
            this.configurationService = configurationService;
            this.dialogService = dialogService;
            this.messagingService = messagingService;

            parserProfileSelected = new Condition(false);

            NewParserProfileCommand = new SimpleCommand((obj) => DoNewParserProfile());
            EditParserProfileCommand = new SimpleCommand((obj) => DoEditParserProfile(), parserProfileSelected);
            DeleteParserProfileCommand = new SimpleCommand((obj) => DoDeleteParserProfile(), parserProfileSelected);
            OkCommand = new SimpleCommand((obj) => DoOk());
            CancelCommand = new SimpleCommand((obj) => DoCancel());

            result = new ModalDialogResult<OpenResult>();


            logSourceViewModels = new ObservableCollection<ILogSourceEditorViewModel>();

            logSourceRepository.LogSourceProviders
                .Select(provider => provider.CreateEditorViewModel())
                .ToList()
                .ForEach(vm => logSourceViewModels.Add(vm));

            bool bestSourceFound = false;

            if (model is OpenFilesModel openFilesModel)
            {
                if (openFilesModel.DroppedFiles != null && openFilesModel.DroppedFiles.Count > 0)
                {
                    foreach (var viewmodel in logSourceViewModels)
                    {
                        var config = viewmodel.Provider.CreateFromLocalPaths(openFilesModel.DroppedFiles);
                        if (config != null)
                        {
                            viewmodel.LoadConfiguration(config);
                            selectedLogSource = viewmodel;
                            bestSourceFound = true;
                            break;
                        }
                    }
                }
            }
            else if (model is OpenClipboardModel)
            {
                foreach (var viewmodel in logSourceViewModels)
                {
                    var config = viewmodel.Provider.CreateFromClipboard();
                    if (config != null)
                    {
                        viewmodel.LoadConfiguration(config);
                        selectedLogSource = viewmodel;
                        bestSourceFound = true;
                        break;
                    }
                }
            }

            if (!bestSourceFound)
                selectedLogSource = logSourceViewModels.FirstOrDefault();

            logParserProfiles = new ObservableCollection<LogParserProfileInfo>();
            BuildLogParserProfileInfos();

            LogParserProfileInfo lastProfile = logParserProfiles.FirstOrDefault(p => p.Guid.Equals(configurationService.Configuration.Session.LastParserProfile.Value));
            SelectedLogParserProfile = lastProfile ?? logParserProfiles.FirstOrDefault();
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
                parserProfileSelected.Value = value != null;
                OnSelectedParserChanged();
            }
        }

        public ModalDialogResult<OpenResult> Result => result;

        public ICommand NewParserProfileCommand { get; }
        public ICommand EditParserProfileCommand { get; }
        public ICommand DeleteParserProfileCommand { get; }
        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
