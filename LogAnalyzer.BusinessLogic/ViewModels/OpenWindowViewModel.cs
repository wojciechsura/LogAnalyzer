﻿using LogAnalyzer.API.LogSource;
using LogAnalyzer.BusinessLogic.Models.Views.OpenWindow;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using LogAnalyzer.Configuration;
using LogAnalyzer.Models.DialogResults;
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
        private readonly IMessagingService messagingService;

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
                    Name = pp.Name.Value,
                    Guid = pp.Guid.Value
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

        private void DoOk()
        {
            if (selectedLogSource == null || !selectedLogSource.Validate())
            {
                messagingService.Inform("Choose and configure log source first!");
                return;
            }

            if (selectedParserProfile == null)
            {
                messagingService.Inform("Choose parser profile first!");
                return;
            }

            result.DialogResult = true;
            result.Result = new OpenResult(selectedLogSource.BuildConfiguration(), selectedLogSource.Provider, selectedParserProfile.Guid);

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

            NewParserProfileCommand = new SimpleCommand((obj) => DoNewParserProfile());
            EditParserProfileCommand = new SimpleCommand((obj) => DoEditParserProfile());
            OkCommand = new SimpleCommand((obj) => DoOk());
            CancelCommand = new SimpleCommand((obj) => DoCancel());

            result = new ModalDialogResult<OpenResult>();

            logSourceViewModels = new ObservableCollection<ILogSourceEditorViewModel>();

            logSourceRepository.LogSourceProviders
                .Select(provider => provider.CreateEditorViewModel())
                .ToList()
                .ForEach(vm => logSourceViewModels.Add(vm));
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

        public ICommand OkCommand { get; private set; }

        public ICommand CancelCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
