using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.Services.Common;
using LogAnalyzer.Services.Models.DialolgResults;
using LogAnalyzer.Services.Interfaces;
using LogAnalyzer.Configuration;
using System.ComponentModel;
using System.Collections.ObjectModel;
using LogAnalyzer.API.LogParser;
using System.Windows.Input;
using LogAnalyzer.Wpf.Input;

namespace LogAnalyzer.BusinessLogic.ViewModels
{
    public class ParserProfileEditorWindowViewModel : INotifyPropertyChanged
    {
        // Private fields -----------------------------------------------------

        private readonly IParserProfileEditorWindowAccess access;
        private readonly ModalDialogResult<LogParserProfileEditorResult> result;
        private readonly ILogParserRepository logParserRepository;
        private readonly IConfigurationService configurationService;
        private readonly IMessagingService messagingService;

        private ObservableCollection<ILogParserEditorViewModel> logParserEditorViewModels;
        private ILogParserEditorViewModel selectedLogParserViewModel;

        private Guid? editedProfileGuid;
        private string profileName;

        // Private methods ----------------------------------------------------

        private void FillProfileFields(LogParserProfile profile)
        {
            profile.Name.Value = profileName;
            profile.ParserUniqueName.Value = selectedLogParserViewModel.Provider.UniqueName;
            profile.SerializedProfile.Value = selectedLogParserViewModel.Provider.SerializeConfiguration(selectedLogParserViewModel.GetConfiguration());
        }

        private void Save()
        {
            if (editedProfileGuid != null)
            {
                var editedProfile = GetEditedProfile(editedProfileGuid);

                configurationService.Configuration.SuspendNotifications();
                try
                {
                    FillProfileFields(editedProfile);
                }
                finally
                {
                    configurationService.Configuration.ResumeNotifications();
                }

                result.DialogResult = true;
                result.Result = new LogParserProfileEditorResult(editedProfileGuid.Value);
            }
            else
            {
                configurationService.Configuration.SuspendNotifications();
                try
                {
                    var newProfile = new LogParserProfile();
                    newProfile.Guid.Value = Guid.NewGuid();
                    FillProfileFields(newProfile);

                    configurationService.Configuration.LogParserProfiles.Add(newProfile);
                }
                finally
                {
                    configurationService.Configuration.ResumeNotifications();
                }

                result.DialogResult = true;
                result.Result = new LogParserProfileEditorResult(editedProfileGuid.Value);
            }
        }

        private void OnProfileNameChanged()
        {
            OnPropertyChanged(nameof(ProfileName));
        }

        private void OnSelectedLogParserViewModelChanged()
        {
            OnPropertyChanged(nameof(SelectedLogParserViewModel));
        }

        private void DoOk()
        {
            if (!selectedLogParserViewModel.Validate())
            {
                messagingService.Inform("Fix all errors in log parser configuration first!");
                return;
            }

            Save();

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

        public ParserProfileEditorWindowViewModel(IParserProfileEditorWindowAccess access, 
            Guid? editedProfileGuid, 
            ILogParserRepository logParserRepository,
            IConfigurationService configurationService,
            IMessagingService messagingService)
        {
            this.access = access;
            this.editedProfileGuid = editedProfileGuid;
            this.logParserRepository = logParserRepository;
            this.configurationService = configurationService;
            this.messagingService = messagingService;

            result = new ModalDialogResult<LogParserProfileEditorResult>();

            LogParserProfile editedProfile = GetEditedProfile(editedProfileGuid, configurationService);

            // Name
            ProfileName = editedProfile?.Name.Value ?? "New profile";

            // Viewmodel
            logParserEditorViewModels = new ObservableCollection<ILogParserEditorViewModel>();
            SelectedLogParserViewModel = null;
            foreach (var provider in logParserRepository.LogParserProviders)
            {
                var vm = provider.CreateEditorViewModel();
                logParserEditorViewModels.Add(vm);

                if (editedProfile != null && provider.UniqueName == editedProfile.ParserUniqueName.Value)
                {
                    ILogParserConfiguration configuration = provider.DeserializeConfiguration(editedProfile.SerializedProfile.Value);
                    vm.SetConfiguration(configuration);
                    SelectedLogParserViewModel = vm;
                }
            }

            OkCommand = new SimpleCommand((obj) => DoOk());
            CancelCommand = new SimpleCommand((obj) => DoCancel());
        }

        private static LogParserProfile GetEditedProfile(Guid? editedProfileGuid)
        {
            LogParserProfile profile = null;
            if (editedProfileGuid != null)
            {
                // Load profile's settings
                profile = configurationService.Configuration.LogParserProfiles
                    .Where(p => p.Guid.Equals(editedProfileGuid))
                    .SingleOrDefault();
            }

            return profile;
        }

        // Public properties --------------------------------------------------

        public ModalDialogResult<LogParserProfileEditorResult> Result => result;

        public string ProfileName
        {
            get
            {
                return profileName;
            }
            set
            {
                profileName = value;
                OnProfileNameChanged();
            }
        }

        public ObservableCollection<ILogParserEditorViewModel> LogParserEditorViewModels => logParserEditorViewModels;

        public ILogParserEditorViewModel SelectedLogParserViewModel
        {
            get
            {
                return selectedLogParserViewModel;
            }
            set
            {
                selectedLogParserViewModel = value;
                OnSelectedLogParserViewModelChanged();
            }
        }

        public ICommand OkCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
