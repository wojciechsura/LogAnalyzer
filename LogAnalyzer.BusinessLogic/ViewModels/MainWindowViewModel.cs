﻿using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;
using Unity.Resolution;
using LogAnalyzer.Dependencies;
using LogAnalyzer.Wpf.Input;
using LogAnalyzer.Services.Interfaces;
using LogAnalyzer.API.LogParser;
using LogAnalyzer.API.LogSource;
using LogAnalyzer.Configuration;
using LogAnalyzer.Models.Engine;
using LogAnalyzer.Types;
using System.ComponentModel;
using LogAnalyzer.Models.DialogResults;

namespace LogAnalyzer.BusinessLogic.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private class CloseData
        {
            public bool HandlingClosing { get; set; } = false;
        }

        // Private fields -----------------------------------------------------
        
        private readonly IMainWindowAccess access;
        private readonly IDialogService dialogService;
        private readonly ILogSourceRepository logSourceRepository;
        private readonly ILogParserRepository logParserRepository;
        private readonly IConfigurationService configurationService;
        private readonly IEngineFactory engineFactory;

        private IEngine engine;

        private Condition engineStoppingCondition;
        private BaseCondition generalCommandCondition;

        // Private methods ----------------------------------------------------

        private void DoCreateEngine(OpenResult result)
        {
            // Build log parser
            LogParserProfile profile = configurationService.Configuration.LogParserProfiles.Single(p => p.Guid.Value.Equals(result.ParserProfileGuid));
            ILogParserProvider logParserProvider = logParserRepository.LogParserProviders.Single(p => p.UniqueName == profile.ParserUniqueName.Value);
            ILogParser parser = logParserProvider.CreateParser(logParserProvider.DeserializeConfiguration(profile.SerializedParserConfiguration.Value));

            // Build log source
            ILogSourceProvider logSourceProvider = logSourceRepository.LogSourceProviders.Single(p => p.UniqueName == result.LogSourceProviderName);
            ILogSource source = logSourceProvider.CreateLogSource(result.LogSourceConfiguration, parser);

            engine = engineFactory.CreateEngine(source, parser);
            engine.NotifySourceReady();

            LogEntries = engine.LogEntries;
            OnPropertyChanged(nameof(LogEntries));
        }

        private void EngineStoppedWithOpenCallback(OpenResult result)
        {
            engineStoppingCondition.Value = false;
            DoCreateEngine(result);
        }

        private void EngineStoppedWithCloseCallback(CloseData closeData)
        {
            engine = null;
            if (!closeData.HandlingClosing)
                access.Close();
        }

        private void DoOpen()
        {
            var result = dialogService.OpenLog();

            if (result.DialogResult)
            {
                if (engine != null)
                {
                    engineStoppingCondition.Value = true;
                    engine.Stop(() => EngineStoppedWithOpenCallback(result.Result));
                }
                else
                {
                    DoCreateEngine(result.Result);
                }
            }
        }

        // Protected methods --------------------------------------------------

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Public methods -----------------------------------------------------

        public MainWindowViewModel(IMainWindowAccess access, 
            IDialogService dialogService, 
            IEngineFactory engineFactory, 
            ILogParserRepository logParserRepository, 
            ILogSourceRepository logSourceRepository,
            IConfigurationService configurationService)
        {
            this.access = access;
            this.dialogService = dialogService;
            this.engineFactory = engineFactory;
            this.logParserRepository = logParserRepository;
            this.logSourceRepository = logSourceRepository;
            this.configurationService = configurationService;

            engineStoppingCondition = new Condition(false);
            generalCommandCondition = !engineStoppingCondition;

            OpenCommand = new SimpleCommand((obj) => DoOpen(), generalCommandCondition);
        }

        public bool HandleClosing()
        {
            if (engine != null)
            {
                CloseData closeData = new CloseData
                {
                    HandlingClosing = true
                };

                // Engine may stop synchronously - in such case
                // callback should not call Close again, because
                // it may cause InvalidOperationException
                engine.Stop(() =>
                {
                    EngineStoppedWithCloseCallback(closeData);
                });

                // If engine stops asynchronously, it will still do
                // it in UI thread, and then will call Close.
                closeData.HandlingClosing = false;

                return engine != null;
            }
            else
            {
                return true;
            }
        }

        // Public properties --------------------------------------------------

        public ICommand OpenCommand { get; private set; }

        public ObservableRangeCollection<HighlightedLogEntry> LogEntries { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
