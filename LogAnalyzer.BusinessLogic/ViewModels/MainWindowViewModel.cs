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
using LogAnalyzer.Types;
using System.ComponentModel;
using LogAnalyzer.Models.DialogResults;
using LogAnalyzer.API.Models;
using LogAnalyzer.Models.Views.HighlightConfigWindow;
using LogAnalyzer.Models.Views.FilterConfigWindow;
using LogAnalyzer.Models.Views.FindWindow;
using System.Windows;

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

        private Wpf.Input.Condition engineStoppingCondition;
        private BaseCondition generalCommandCondition;
        private Wpf.Input.Condition enginePresentCondition;
        private BaseCondition generalEnginePresentCondition;

        private bool bottomPaneVisible;
        private HighlightedLogEntry selectedSearchResult;

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
            access.SetupListViews(engine.GetColumnInfos());
            engine.NotifySourceReady();

            LogEntries = engine.LogEntries;
            OnPropertyChanged(nameof(LogEntries));
            SearchResults = engine.SearchResults;
            OnPropertyChanged(nameof(SearchResults));

            enginePresentCondition.Value = true;
            bottomPaneVisible = false;
        }

        private void DoStopEngine()
        {
            engineStoppingCondition.Value = false;
            engine = null;
            access.ClearListView();
        }

        private void EngineStoppedWithOpenCallback(OpenResult result)
        {
            DoStopEngine();
            DoCreateEngine(result);
        }

        private void EngineStoppedWithCloseCallback(CloseData closeData)
        {
            DoStopEngine();
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

        private void DoHighlightConfig()
        {
            HighlightConfigModel model = new HighlightConfigModel(engine.HighlightConfig, engine.GetColumnInfos());
            var result = dialogService.ConfigHighlighting(model);
            if (result.DialogResult)
                engine.HighlightConfig = result.Result;
        }

        private void DoFilterConfig()
        {
            FilterConfigModel model = new FilterConfigModel(engine.FilterConfig, engine.GetColumnInfos());
            var result = dialogService.ConfigFiltering(model);
            if (result.DialogResult)
                engine.FilterConfig = result.Result;
        }

        private void DoSearch()
        {
            List<string> availableCustomColumns = engine.GetColumnInfos()
                .OfType<CustomColumnInfo>()
                .Select(c => c.Name)
                .ToList();

            FindModel model = new FindModel(engine.SearchConfig, availableCustomColumns);
            var result = dialogService.OpenFind(model);
            if (result.DialogResult)
            {
                engine.SearchConfig = result.Result;
                BottomPaneVisible = true;
            }
        }

        private void DoToggleBottomPane()
        {
            BottomPaneVisible = !BottomPaneVisible;
        }

        private void DoCloseBootomPane()
        {
            BottomPaneVisible = false;
        }

        private void DoCopy(IList<object> items)
        {
            if (items.Count == 0)
                return;

            var columns = engine.GetColumnInfos();

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < items.Count; i++)
            {
                var entry = items[i] as HighlightedLogEntry;
                if (entry == null)
                    throw new InvalidOperationException("Invalid selected item!");

                for (int col = 0; col < columns.Count; col++)
                {
                    BaseColumnInfo info = columns[col];
                    if (info is CommonColumnInfo commonInfo)
                    {
                        switch (commonInfo.Column)
                        {
                            case API.Types.LogEntryColumn.Date:
                                builder.Append(entry.LogEntry.DisplayDate);
                                break;
                            case API.Types.LogEntryColumn.Severity:
                                builder.Append(entry.LogEntry.Severity);
                                break;
                            case API.Types.LogEntryColumn.Message:
                                builder.Append(entry.LogEntry.Message);
                                break;
                            case API.Types.LogEntryColumn.Custom:
                                throw new InvalidOperationException("Invalid column type in common column");
                        }
                    }
                    else if (info is CustomColumnInfo customInfo)
                    {
                        builder.Append(entry.LogEntry.CustomFields[customInfo.Index]);
                        
                    }
                    if (col < columns.Count - 1)
                        builder.Append("\t");
                    else
                        builder.Append("\n");
                }
            }

            Clipboard.SetText(builder.ToString());
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

            engineStoppingCondition = new Wpf.Input.Condition(false);
            generalCommandCondition = !engineStoppingCondition;
            enginePresentCondition = new Wpf.Input.Condition(false);
            generalEnginePresentCondition = enginePresentCondition & !engineStoppingCondition;

            OpenCommand = new SimpleCommand((obj) => DoOpen(), generalCommandCondition);
            HighlightConfigCommand = new SimpleCommand((obj) => DoHighlightConfig(), generalEnginePresentCondition);
            FilterConfigCommand = new SimpleCommand((obj) => DoFilterConfig(), generalEnginePresentCondition);
            SearchCommand = new SimpleCommand((obj) => DoSearch(), generalEnginePresentCondition);
            ToggleBottomPaneCommand = new SimpleCommand((obj) => DoToggleBottomPane());
            CloseBottomPaneCommand = new SimpleCommand((obj) => DoCloseBootomPane());
            CopyCommand = new SimpleCommand((obj) => DoCopy((IList<object>)obj), enginePresentCondition);
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
                return false;
            }
        }

        public void OnSearchResultChosen()
        {
            if (selectedSearchResult != null)
                access.NavigateTo(selectedSearchResult);
        }

        // Public properties --------------------------------------------------

        public ICommand OpenCommand { get; }

        public ICommand HighlightConfigCommand { get; }

        public ICommand FilterConfigCommand { get; }

        public ICommand SearchCommand { get; }

        public ICommand ToggleBottomPaneCommand { get; }

        public ICommand CloseBottomPaneCommand { get; }

        public ICommand CopyCommand { get; }

        public ObservableRangeCollection<HighlightedLogEntry> LogEntries { get; private set; }

        public ObservableRangeCollection<HighlightedLogEntry> SearchResults { get; private set; }

        public HighlightedLogEntry SelectedSearchResult { get => selectedSearchResult;
        set
            {
                selectedSearchResult = value;
                OnPropertyChanged(nameof(SelectedSearchResult));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool BottomPaneVisible
        {
            get => bottomPaneVisible;
            set
            {
                bottomPaneVisible = value;
                OnPropertyChanged(nameof(BottomPaneVisible));
            }
        }
    }
}
