using LogAnalyzer.API.LogParser;
using LogAnalyzer.API.LogSource;
using LogAnalyzer.API.Models;
using LogAnalyzer.API.Types;
using LogAnalyzer.BusinessLogic.Models.Views.OpenWindow;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using LogAnalyzer.BusinessLogic.ViewModels.Open;
using LogAnalyzer.Configuration;
using LogAnalyzer.Models.DialogResults;
using LogAnalyzer.Models.Views.JsonCodeWindow;
using LogAnalyzer.Models.Views.OpenWindow;
using LogAnalyzer.Services.Common;
using LogAnalyzer.Services.Interfaces;
using LogAnalyzer.Wpf.Input;
using Newtonsoft.Json;
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
        // Private types ------------------------------------------------------

        private class ParserProfileModel
        {
            public string Name { get; set; }
            public string ParserUniqueName { get; set; }
            public string ParserSettings { get; set; }
        }

        // Private fields -----------------------------------------------------

        private readonly IOpenWindowAccess access;
        private readonly ModalDialogResult<OpenResult> result;
        private readonly ILogSourceRepository logSourceRepository;
        private readonly ILogParserRepository logParserRepository;
        private readonly IConfigurationService configurationService;
        private readonly IDialogService dialogService;
        private readonly IMessagingService messagingService;

        private readonly ObservableCollection<ILogSourceEditorViewModel> logSourceViewModels;
        private ILogSourceEditorViewModel selectedLogSource;

        private readonly ObservableCollection<LogParserProfileInfo> logParserProfiles;
        private LogParserProfileInfo selectedParserProfile;
        private Condition parserProfileSelected;

        private readonly ObservableCollection<ProcessingProfileViewModel> processingProfiles;
        private ProcessingProfileViewModel selectedProcessingProfile;

        private readonly bool detectParsers;

        // Private methods ----------------------------------------------------

        private void BuildLogParserProfileInfos()
        {
            logParserProfiles.Clear();
            configurationService.Configuration.LogParserProfiles
                .Select(pp =>
                {
                    ILogParser parser = null;

                    // Build parsers only if user wants to auto-detect them.
                    // This takes time, might be noticeable if there are plenty of them.
                    if (detectParsers)
                    {
                        var provider = logParserRepository.LogParserProviders.FirstOrDefault(p => p.UniqueName == pp.ParserUniqueName.Value);
                        if (provider != null)
                        {
                            var configuration = provider.DeserializeConfiguration(pp.SerializedParserConfiguration.Value);
                            if (configuration != null)
                            {
                                parser = provider.CreateParser(configuration);
                            }
                        }
                    }

                    return new LogParserProfileInfo(pp.Name.Value, pp.Guid.Value, parser);
                })
                .ToList()
                .ForEach(pp => logParserProfiles.Add(pp));
        }

        private void BuildProcessingProfiles()
        {
            processingProfiles.Clear();

            processingProfiles.Add(new ProcessingProfileViewModel("None", Guid.Empty));

            ConfigurationBase.SimpleCollection<ProcessingProfile> profileRepository = configurationService.Configuration.ProcessingProfiles;
            for (int i = 0; i < profileRepository.Count; i++)
            {
                var profile = profileRepository[i];
                processingProfiles.Add(new ProcessingProfileViewModel(profile.Name.Value, profile.Guid.Value));
            }
        }

        private void SelectBestLogSource(BaseOpenFilesModel model)
        {
            bool bestSourceFound = false;

            if (model is OpenFilesModel openFilesModel)
            {
                if (openFilesModel.DroppedFiles != null && openFilesModel.DroppedFiles.Count > 0)
                {
                    // Find first viewmodel, which can handle dropped multiple files
                    foreach (var viewmodel in logSourceViewModels)
                    {
                        var config = viewmodel.Provider.CreateFromLocalPaths(openFilesModel.DroppedFiles);
                        if (config != null)
                        {
                            viewmodel.LoadConfiguration(config);
                            SetLogSource(viewmodel);
                            bestSourceFound = true;
                            break;
                        }
                    }
                }
            }
            else if (model is OpenClipboardModel)
            {
                // Find first viewmodel, which can handle providing data from clipboard
                foreach (var viewmodel in logSourceViewModels)
                {
                    var config = viewmodel.Provider.CreateFromClipboard();
                    if (config != null)
                    {
                        viewmodel.LoadConfiguration(config);
                        SetLogSource(viewmodel);
                        bestSourceFound = true;
                        break;
                    }
                }
            }

            if (!bestSourceFound)
                SetLogSource(logSourceViewModels.FirstOrDefault());
        }

        private void BuildLogSourceViewModels(ILogSourceRepository logSourceRepository)
        {
            logSourceRepository.LogSourceProviders
                .Select(provider => provider.CreateEditorViewModel())
                .ToList()
                .ForEach(vm => logSourceViewModels.Add(vm));
        }

        private List<string> GetSampleLinesFromSource()
        {
            if (selectedLogSource.ProvidesSampleLines)
                return selectedLogSource.ProvideSampleLines();
            else
                return new List<string>();
        }

        private void SetLogSource(ILogSourceEditorViewModel value)
        {
            UnsubscribeLogSource(selectedLogSource);

            selectedLogSource = value;
            OnPropertyChanged(nameof(SelectedLogSource));

            SubscribeLogSource(selectedLogSource);

            DoHandleSourceChanged();
        }

        private void SubscribeLogSource(ILogSourceEditorViewModel logSource)
        {
            if (logSource != null)
                logSource.SourceChanged += HandleSourceChanged;
        }

        private void UnsubscribeLogSource(ILogSourceEditorViewModel logSource)
        {
            if (logSource != null)
                logSource.SourceChanged -= HandleSourceChanged;
        }

        private void HandleSourceChanged(object sender, EventArgs e)
        {
            DoHandleSourceChanged();
        }

        private void DoHandleSourceChanged()
        {
            CheckCompatibleParsers();
        }

        private void OnSelectedParserChanged()
        {
            OnPropertyChanged(nameof(SelectedLogParserProfile));
        }

        private void CheckCompatibleParsers()
        {
            if (detectParsers && selectedLogSource != null && selectedLogSource.ProvidesSampleLines)
            {
                List<string> sampleLines = selectedLogSource.ProvideSampleLines();

                for (int parser = 0; parser < logParserProfiles.Count; parser++)
                {
                    logParserProfiles[parser].Compatible = sampleLines
                        .FirstOrDefault(s =>
                        {
                            (BaseLogEntry entry, ParserOperation op) = logParserProfiles[parser].Parser.Parse(s, null);

                            return (entry != null && op == ParserOperation.AddNew);
                        }) != null;
                }
            }
            else
            {
                for (int i = 0; i < logParserProfiles.Count; i++)
                {
                    logParserProfiles[i].Compatible = false;
                }
            }
        }

        private void HandleParserProfilesChanged(Guid newParserGuid)
        {
            BuildLogParserProfileInfos();
            CheckCompatibleParsers();

            SelectedLogParserProfile = logParserProfiles
                .Where(p => p.Guid.Equals(newParserGuid))
                .FirstOrDefault();
        }

        private void DoEditParserProfile()
        {
            var sampleLines = GetSampleLinesFromSource();

            ModalDialogResult<LogParserProfileEditorResult> result = dialogService.EditLogParserProfile(selectedParserProfile.Guid, sampleLines);
            if (result.DialogResult)
                HandleParserProfilesChanged(result.Result.Guid);
        }

        private void DoNewParserProfile()
        {
            var sampleLines = GetSampleLinesFromSource();

            ModalDialogResult<LogParserProfileEditorResult> result = dialogService.NewLogParserProfile(sampleLines);
            if (result.DialogResult)
                HandleParserProfilesChanged(result.Result.Guid);
        }

        private void DoDeleteParserProfile()
        {
            if (messagingService.Ask("Are you sure you want to delete this profile?"))
            {
                var deletedProfile = configurationService.Configuration.LogParserProfiles.Where(p => p.Guid.Value == selectedParserProfile.Guid).Single();
                configurationService.Configuration.LogParserProfiles.Remove(deletedProfile);

                logParserProfiles.Remove(selectedParserProfile);
                SelectedLogParserProfile = logParserProfiles.FirstOrDefault();

                // No need to re-check compatible parsers here
            }
        }

        private ParserProfileModel DeserializeParserModel(string code)
        {
            try
            {
                return JsonConvert.DeserializeObject<ParserProfileModel>(code);
            }
            catch
            {                
                return null;
            }
        }

        private bool VerifyParserProfileModel(ParserProfileModel model)
        {
            var parser = logParserRepository.LogParserProviders.FirstOrDefault(pp => pp.UniqueName == model.ParserUniqueName);
            if (parser == null)
            {
                return false;
            }

            ILogParserConfiguration configuration;
            try
            {
                configuration = parser.DeserializeConfiguration(model.ParserSettings);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private void AddParserProfile(ParserProfileModel parserModel)
        {
            configurationService.Configuration.SuspendNotifications();

            var newProfileGuid = Guid.NewGuid();

            try
            {
                var newProfile = new LogParserProfile();

                newProfile.Guid.Value = newProfileGuid;
                newProfile.Name.Value = parserModel.Name;
                newProfile.ParserUniqueName.Value = parserModel.ParserUniqueName;
                newProfile.SerializedParserConfiguration.Value = parserModel.ParserSettings;

                configurationService.Configuration.LogParserProfiles.Add(newProfile);
            }
            finally
            {
                configurationService.Configuration.ResumeNotifications();
            }

            HandleParserProfilesChanged(newProfileGuid);
        }

        private void DoImportProfile()
        {
            JsonCodeModel model = new JsonCodeModel
            {
                Title = "Import parser profile",
                Hint = "Paste parser code here:",
                ShowCancel = true
            };

            var result = dialogService.OpenJsonCodeWindow(model);
            if (result.DialogResult)
            {
                // Deserialize

                ParserProfileModel parserModel = DeserializeParserModel(result.Result.Code);

                if (parserModel == null)
                {
                    messagingService.Warn("Invalid exported parser string!");
                    return;
                }

                // Verify

                if (!VerifyParserProfileModel(parserModel))
                {
                    messagingService.Warn("Invalid exported parser string!");
                    return;
                }

                // Add

                AddParserProfile(parserModel);

                messagingService.Inform("Successfully imported parser profile!");
            }
        }

        private void DoExportProfile()
        {
            // Find selected profile

            var profile = configurationService.Configuration.LogParserProfiles
                .Single(pp => pp.Guid.Value.Equals(selectedParserProfile.Guid));

            // Create model

            ParserProfileModel parserModel = new ParserProfileModel
            {
                Name = profile.Name.Value,
                ParserUniqueName = profile.ParserUniqueName.Value,
                ParserSettings = profile.SerializedParserConfiguration.Value
            };

            // Serialize to JSON

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
            var code = JsonConvert.SerializeObject(parserModel, Formatting.None, settings);

            // Display

            JsonCodeModel model = new JsonCodeModel
            {
                Code = code,
                Title = "Export parser profile",
                Hint = "Copy the following code:",
                ShowCancel = false
            };
            dialogService.OpenJsonCodeWindow(model);
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
            result.Result = new OpenResult(selectedLogSource.BuildConfiguration(), selectedLogSource.Provider.UniqueName, selectedParserProfile.Guid, selectedProcessingProfile?.Guid ?? Guid.Empty);

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

            this.detectParsers = configurationService.Configuration.OpeningConfiguration.DetectParsers.Value;

            // Conditions

            parserProfileSelected = new Condition(false);

            // Commands

            NewParserProfileCommand = new SimpleCommand((obj) => DoNewParserProfile());
            EditParserProfileCommand = new SimpleCommand((obj) => DoEditParserProfile(), parserProfileSelected);
            DeleteParserProfileCommand = new SimpleCommand((obj) => DoDeleteParserProfile(), parserProfileSelected);
            ExportProfileCommand = new SimpleCommand((obj) => DoExportProfile(), parserProfileSelected);
            ImportProfileCommand = new SimpleCommand((obj) => DoImportProfile());
            OkCommand = new SimpleCommand((obj) => DoOk());
            CancelCommand = new SimpleCommand((obj) => DoCancel());

            // Result

            result = new ModalDialogResult<OpenResult>();

            // Log profiles

            logParserProfiles = new ObservableCollection<LogParserProfileInfo>();
            BuildLogParserProfileInfos();

            LogParserProfileInfo lastProfile = logParserProfiles.FirstOrDefault(p => p.Guid.Equals(configurationService.Configuration.Session.LastParserProfile.Value));
            SelectedLogParserProfile = lastProfile ?? logParserProfiles.FirstOrDefault();

            // Log sources

            logSourceViewModels = new ObservableCollection<ILogSourceEditorViewModel>();
            BuildLogSourceViewModels(logSourceRepository);

            // Processing profiles

            processingProfiles = new ObservableCollection<ProcessingProfileViewModel>();
            BuildProcessingProfiles();
            selectedProcessingProfile = processingProfiles.FirstOrDefault();

            // Select best matching log source - only after everything else is ready

            SelectBestLogSource(model);
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
                SetLogSource(value);
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

        public ObservableCollection<ProcessingProfileViewModel> ProcessingProfiles => processingProfiles;
        public ProcessingProfileViewModel SelectedProcessingProfile
        {
            get => selectedProcessingProfile;
            set
            {
                selectedProcessingProfile = value;
                OnPropertyChanged(nameof(SelectedProcessingProfile));
            }
        }

        public ModalDialogResult<OpenResult> Result => result;

        public ICommand NewParserProfileCommand { get; }
        public ICommand EditParserProfileCommand { get; }
        public ICommand DeleteParserProfileCommand { get; }
        public ICommand ExportProfileCommand { get; }
        public ICommand ImportProfileCommand { get; }
        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
