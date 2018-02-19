using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
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
using LogAnalyzer.Models.Views.OpenWindow;
using LogAnalyzer.Models.Views.JumpToTime;
using LogAnalyzer.Models.Views.ColumnSelectionWindow;
using LogAnalyzer.Models.Engine;
using LogAnalyzer.Models.Engine.PredicateDescriptions;
using LogAnalyzer.Models.Types;
using LogAnalyzer.Models.Views.NoteWindow;

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
        private Wpf.Input.Condition itemSelectedCondition;

        private bool bottomPaneVisible;
        private LogRecord selectedSearchResult;
        private LogRecord selectedLogEntry;

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

        private void DoOpen(List<string> droppedFiles = null)
        {
            var result = dialogService.OpenLog(new OpenFilesModel { DroppedFiles = droppedFiles });

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

        private void OpenHighlightConfig(HighlightEntry newEntry = null)
        {
            HighlightConfigModel model = new HighlightConfigModel(engine.HighlightConfig, engine.GetColumnInfos(), newEntry);
            var result = dialogService.ConfigHighlighting(model);
            if (result.DialogResult)
                engine.HighlightConfig = result.Result;
        }

        private void OpenFilterConfig(FilterEntry newEntry = null)
        {
            FilterConfigModel model = new FilterConfigModel(engine.FilterConfig, engine.GetColumnInfos(), newEntry);
            var result = dialogService.ConfigFiltering(model);
            if (result.DialogResult)
                engine.FilterConfig = result.Result;
        }

        private void DoHighlightConfig()
        {
            OpenHighlightConfig();
        }

        private void DoFilterConfig()
        {
            OpenFilterConfig(null);
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

        private void DoCopy()
        {
            System.Collections.IList items = access.GetMainSelectedItems();

            if (items.Count == 0)
                return;

            var columns = engine.GetColumnInfos();

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < items.Count; i++)
            {
                var entry = items[i] as LogRecord;
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

        private void DoJumpToTime()
        {
            DateTime firstTime = engine.GetFirstFilteredTime();
            JumpToTimeModel model = new JumpToTimeModel { DefaultDate = firstTime };
            var result = dialogService.OpenJumpToTime(model);
            if (result.DialogResult)
            {
                var entry = engine.FindFirstRecordAfter(result.Result.ResultDate);
                if (entry != null)
                {
                    access.NavigateTo(entry);
                }
            }
        }

        private void DoGotoBookmark(string id)
        {
            LogRecord bookmarkedRecord = engine.GetLogRecordForBookmark(id);
            access.NavigateTo(bookmarkedRecord);
        }

        private void DoSetBookmark(string id)
        {
            if (selectedLogEntry == null)
                throw new InvalidOperationException("Cannot set bookmark - no entry selected!");

            engine.AddBookmark(id, selectedLogEntry);
        }

        private void DoAddFilteringRule()
        {
            var model = new ColumnSelectionModel(engine.GetColumnInfos());
            var result = dialogService.SelectColumn(model);
            if (result.DialogResult)
            {
                FilterEntry entry = CreateFilterEntryFromSelectedEntry(result.Result.SelectedColumn);

                OpenFilterConfig(entry);
            }
        }

        private void DoAddHighlightingRule()
        {
            var model = new ColumnSelectionModel(engine.GetColumnInfos());
            var result = dialogService.SelectColumn(model);
            if (result.DialogResult)
            {
                HighlightEntry entry = CreateHighlightEntryFromSelectedEntry(result.Result.SelectedColumn);

                OpenHighlightConfig(entry);
            }
        }

        private HighlightEntry CreateHighlightEntryFromSelectedEntry(BaseColumnInfo columnInfo)
        {
            PredicateDescription predicate = CreatePredicateFromSelectedEntry(columnInfo);

            return new HighlightEntry
            {
                PredicateDescription = predicate
            };
        }

        private FilterEntry CreateFilterEntryFromSelectedEntry(BaseColumnInfo columnInfo)
        {
            PredicateDescription predicate = CreatePredicateFromSelectedEntry(columnInfo);

            return new FilterEntry
            {
                PredicateDescription = predicate
            };
        }

        private PredicateDescription CreatePredicateFromSelectedEntry(BaseColumnInfo columnInfo)
        {
            if (columnInfo is CommonColumnInfo commonColumn)
            {
                switch (commonColumn.Column)
                {
                    case API.Types.LogEntryColumn.Date:
                        {
                            return new DatePredicateDescription
                            {
                                Argument = selectedLogEntry.LogEntry.Date,
                                Comparison = ComparisonMethod.MoreThan
                            };
                        }
                    case API.Types.LogEntryColumn.Severity:
                        {
                            return new SeverityPredicateDescription
                            {
                                Argument = selectedLogEntry.LogEntry.Severity,
                                Comparison = ComparisonMethod.Contains
                            };
                        }
                    case API.Types.LogEntryColumn.Message:
                        {
                            return new MessagePredicateDescription
                            {
                                Argument = selectedLogEntry.LogEntry.Message,
                                Comparison = ComparisonMethod.Contains
                            };
                        }
                    case API.Types.LogEntryColumn.Custom:
                        throw new InvalidOperationException("Custom column should not be present in CommonColumnInfo!");
                    default:
                        throw new InvalidOperationException("Unsupported column!");
                }
            }
            else if (columnInfo is CustomColumnInfo customColumn)
            {
                return new CustomPredicateDescription
                {
                    Argument = selectedLogEntry.LogEntry.CustomFields[customColumn.Index],
                    Comparison = ComparisonMethod.Contains,
                    Name = customColumn.Name
                };
            }
            else
                throw new InvalidOperationException("Invalid column info!");
        }

        private void DoToggleProfilingPointCommand()
        {
            if (engine.IsProfilingEntry(selectedLogEntry.LogEntry))
                engine.RemoveProfilingEntry(selectedLogEntry.LogEntry);
            else
                engine.AddProfilingEntry(selectedLogEntry.LogEntry);
        }

        private void DoClearProfilingPointsCommand()
        {
            engine.ClearProfilingEntries();
        }

        private void DoAnnotate()
        {
            NoteModel model = new NoteModel { Note = engine.GetNote(selectedLogEntry) };
            var result = dialogService.EditAnnotations(model);
            if (result.DialogResult)
            {
                engine.AddNote(result.Result.Note, selectedLogEntry);
            }
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string name)
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
            itemSelectedCondition = new Wpf.Input.Condition(false);

            OpenCommand = new SimpleCommand((obj) => DoOpen(), generalCommandCondition);
            HighlightConfigCommand = new SimpleCommand((obj) => DoHighlightConfig(), generalEnginePresentCondition);
            FilterConfigCommand = new SimpleCommand((obj) => DoFilterConfig(), generalEnginePresentCondition);
            SearchCommand = new SimpleCommand((obj) => DoSearch(), generalEnginePresentCondition);
            ToggleBottomPaneCommand = new SimpleCommand((obj) => DoToggleBottomPane());
            CloseBottomPaneCommand = new SimpleCommand((obj) => DoCloseBootomPane());
            CopyCommand = new SimpleCommand((obj) => DoCopy(), enginePresentCondition);
            JumpToTimeCommand = new SimpleCommand((obj) => DoJumpToTime(), enginePresentCondition);
            SetBookmarkCommand = new SimpleCommand((obj) => DoSetBookmark((string)obj), enginePresentCondition & itemSelectedCondition);
            GotoBookmarkCommand = new SimpleCommand((obj) => DoGotoBookmark((string)obj), enginePresentCondition);
            AddHighlightingRuleCommand = new SimpleCommand((obj) => DoAddHighlightingRule(), enginePresentCondition & itemSelectedCondition);
            AddFilteringRuleCommand = new SimpleCommand((obj) => DoAddFilteringRule(), enginePresentCondition & itemSelectedCondition);
            ToggleProfilingPointCommand = new SimpleCommand((obj) => DoToggleProfilingPointCommand(), enginePresentCondition & itemSelectedCondition);
            ClearProfilingPointsCommand = new SimpleCommand((obj) => DoClearProfilingPointsCommand(), enginePresentCondition);
            AnnotateCommand = new SimpleCommand((obj) => DoAnnotate(), enginePresentCondition & itemSelectedCondition);
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

        public void FilesDropped(List<string> files)
        {
            DoOpen(files);
        }

        // Public properties --------------------------------------------------

        public ICommand OpenCommand { get; }

        public ICommand HighlightConfigCommand { get; }

        public ICommand FilterConfigCommand { get; }

        public ICommand SearchCommand { get; }

        public ICommand ToggleBottomPaneCommand { get; }

        public ICommand CloseBottomPaneCommand { get; }

        public ICommand CopyCommand { get; }

        public ICommand JumpToTimeCommand { get; }

        public ICommand SetBookmarkCommand { get; }

        public ICommand GotoBookmarkCommand { get; }

        public ICommand AddHighlightingRuleCommand { get; }

        public ICommand AddFilteringRuleCommand { get; }

        public ICommand ToggleProfilingPointCommand { get; }

        public ICommand ClearProfilingPointsCommand { get; }

        public ICommand AnnotateCommand { get; }

        public ObservableRangeCollection<LogRecord> LogEntries { get; private set; }

        public ObservableRangeCollection<LogRecord> SearchResults { get; private set; }

        public LogRecord SelectedSearchResult
        {
            get => selectedSearchResult;
            set
            {
                selectedSearchResult = value;
                OnPropertyChanged(nameof(SelectedSearchResult));
            }
        }

        public int SelectedEntryIndex
        {
            set
            {
                itemSelectedCondition.Value = value != -1;
            }
        }

        public LogRecord SelectedLogEntry
        {
            get => selectedLogEntry;
            set
            {
                selectedLogEntry = value;
                OnPropertyChanged(nameof(SelectedLogEntry));
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
