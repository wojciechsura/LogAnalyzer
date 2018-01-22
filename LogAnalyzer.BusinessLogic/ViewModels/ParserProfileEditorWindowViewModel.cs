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

namespace LogAnalyzer.BusinessLogic.ViewModels
{
    public class ParserProfileEditorWindowViewModel : INotifyPropertyChanged
    {
        // Private fields -----------------------------------------------------

        private readonly IParserProfileEditorWindowAccess access;
        private readonly ModalDialogResult<LogParserProfileEditorResult> result;
        private readonly ILogParserRepository logParserRepository;
        private readonly IConfigurationService configurationService;

        private ObservableCollection<ILogParserEditorViewModel> logParserEditorViewModels;
        private ILogParserEditorViewModel selectedLogParserViewModel;

        private Guid? editedProfileGuid;
        private string profileName;

        // Private methods ----------------------------------------------------

        private void OnProfileNameChanged()
        {
            OnPropertyChanged(nameof(ProfileName));
        }

        private void OnSelectedLogParserViewModelChanged()
        {
            OnPropertyChanged(nameof(SelectedLogParserViewModel));
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
            IConfigurationService configurationService)
        {
            this.access = access;
            this.editedProfileGuid = editedProfileGuid;
            this.logParserRepository = logParserRepository;
            this.configurationService = configurationService;

            result = new ModalDialogResult<LogParserProfileEditorResult>();

            LogParserProfile editedProfile = GetEditedProfile(editedProfileGuid, configurationService);

            // Name
            ProfileName = editedProfile?.Name ?? "New profile";

            logParserEditorViewModels = new ObservableCollection<ILogParserEditorViewModel>();
            SelectedLogParserViewModel = null;
            foreach (var provider in logParserRepository.LogParserProviders)
            {
                var vm = provider.CreateEditorViewModel();
                logParserEditorViewModels.Add(vm);

                if (editedProfile != null && provider.UniqueName == editedProfile.ParserUniqueName)
                {
                    ILogParserConfiguration configuration = provider.DeserializeConfiguration(editedProfile.SerializedProfile);
                    vm.SetConfiguration(configuration);
                    SelectedLogParserViewModel = vm;
                }
            }               
        }

        private static LogParserProfile GetEditedProfile(Guid? editedProfileGuid, IConfigurationService configurationService)
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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
