﻿using ConfigurationBase;
using ICSharpCode.AvalonEdit.Document;
using IronPython.Hosting;
using LogAnalyzer.API.LogParser;
using LogAnalyzer.API.LogSource;
using LogAnalyzer.API.Models;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using LogAnalyzer.BusinessLogic.ViewModels.Main;
using LogAnalyzer.BusinessLogic.ViewModels.Scripting;
using LogAnalyzer.Configuration;
using LogAnalyzer.Dependencies;
using LogAnalyzer.Models.DialogResults;
using LogAnalyzer.Models.Engine.PredicateDescriptions;
using LogAnalyzer.Models.Engine;
using LogAnalyzer.Models.Events;
using LogAnalyzer.Models.Types;
using LogAnalyzer.Models.Views.ColumnSelectionWindow;
using LogAnalyzer.Models.Views.FilterConfigWindow;
using LogAnalyzer.Models.Views.FindWindow;
using LogAnalyzer.Models.Views.HighlightConfigWindow;
using LogAnalyzer.Models.Views.JumpToTime;
using LogAnalyzer.Models.Views.LogMessageVisualizerWindow;
using LogAnalyzer.Models.Views.NoteWindow;
using LogAnalyzer.Models.Views.OpenWindow;
using LogAnalyzer.Models.Views.ProcessingProfileNameWindow;
using LogAnalyzer.Scripting.ScriptAPI;
using LogAnalyzer.Scripting;
using LogAnalyzer.Services.Interfaces;
using LogAnalyzer.Types;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting;
using Newtonsoft.Json;
using Spooksoft.VisualStateManager.Commands;
using Spooksoft.VisualStateManager.Conditions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System;

namespace LogAnalyzer.BusinessLogic.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged, IScriptingHost, IEventListener<ProcessingProfileListChanged>, IEventListener<StoredScriptListChanged>
    {
        // Private constants --------------------------------------------------

        private const string WINDOW_TITLE = "Spooksoft LogAnalyzer";

        // Private types ------------------------------------------------------

        private class CloseData
        {
            public bool HandlingClosing { get; set; } = false;
        }

        private class PythonScriptErrorListener : ErrorListener
        {
            private Action<ScriptSource, string, SourceSpan, int, Severity> errorAction;

            public override void ErrorReported(ScriptSource source, string message, SourceSpan span, int errorCode, Severity severity)
            {
                errorAction(source, message, span, errorCode, severity);
            }

            public PythonScriptErrorListener(Action<ScriptSource, string, SourceSpan, int, Severity> errorAction)
            {
                this.errorAction = errorAction ?? throw new ArgumentNullException("errorAction");
            }
        }

        // Private fields -----------------------------------------------------

        private readonly IMainWindowAccess access;
        private readonly IDialogService dialogService;
        private readonly ILogSourceRepository logSourceRepository;
        private readonly ILogParserRepository logParserRepository;
        private readonly IConfigurationService configurationService;
        private readonly IEngineFactory engineFactory;
        private readonly ITextParser textParser;
        private readonly IMessagingService messagingService;
        private readonly IWinApiService winApiService;
        private readonly IExportService exportService;
        private readonly IEventBusService eventBusService;

        private IEngine engine;

        private readonly SimpleCondition engineStoppingCondition;
        private readonly BaseCondition generalCommandCondition;
        private readonly SimpleCondition enginePresentCondition;
        private readonly BaseCondition generalEnginePresentCondition;
        private readonly SimpleCondition itemSelectedCondition;
        private readonly SimpleCondition searchStringExists;
        private readonly SimpleCondition profileSelectedCondition;
        private readonly SimpleCondition firstProfileSelectedCondition;
        private readonly SimpleCondition lastProfileSelectedCondition;
        private readonly SimpleCondition scriptSelectedCondition;
        private readonly SimpleCondition firstScriptSelectedCondition;
        private readonly SimpleCondition lastScriptSelectedCondition;
        private readonly BaseCondition scriptRunCondition;

        private bool bottomPaneVisible;
        private int bottomPaneSelectedTabIndex;
        private bool rightPaneVisible;
        private int rightPaneSelectedTabIndex;
        private LogRecord selectedSearchResult;
        private LogRecord selectedLogEntry;

        private string searchString;
        private bool searchBoxVisible;
        private bool searchCaseSensitive;
        private bool searchWholeWords;
        private bool searchRegex;

        private string title = WINDOW_TITLE;

        private TextDocument scriptLogDocument;

        private readonly ObservableCollection<ProcessingProfileViewModel> processingProfiles;
        private readonly ICommand processingProfileClickCommand;
        private ProcessingProfileViewModel selectedProcessingProfile;
        private int selectedProcessingProfileIndex;

        private readonly ObservableCollection<StoredScriptViewModel> storedScripts;
        private readonly ICommand storedScriptClickCommand;
        private StoredScriptViewModel selectedStoredScript;
        private int selectedStoredScriptIndex;

        private bool loadingStatus;
        private string loadingStatusText;
        private bool processingStatus;
        private string processingStatusText;

        // Private methods ----------------------------------------------------

        private void HandleEngineProcessingStatusChanged(object sender, StatusChangedEventArgs args)
        {
            ProcessingStatus = args.Status;
        }

        private void HandleEngineLoadingStatusChanged(object sender, StatusChangedEventArgs args)
        {
            LoadingStatus = args.Status;
        }

        private void BuildProcessingProfiles()
        {
            processingProfiles.Clear();

            var profiles = configurationService.Configuration.ProcessingProfiles;
            for (int i = 0; i < profiles.Count; i++)
            {
                var profile = profiles[i];

                ProcessingProfileViewModel model = new ProcessingProfileViewModel(profile.Name.Value, profile.Guid.Value, processingProfileClickCommand);
                processingProfiles.Add(model);
            }
        }

        private void BuildStoredScripts()
        {
            storedScripts.Clear();

            var scripts = configurationService.Configuration.StoredScripts;
            for (int i = 0; i < scripts.Count; i++)
            {
                var script = scripts[i];

                StoredScriptViewModel model = new StoredScriptViewModel(script.Name.Value, script.Guid.Value, storedScriptClickCommand);
                storedScripts.Add(model);
            }
        }

        private void DoCreateEngine(OpenResult result)
        {
            // Build log parser
            LogParserProfile profile = configurationService.Configuration.LogParserProfiles.Single(p => p.Guid.Value.Equals(result.ParserProfileGuid));
            ILogParserProvider logParserProvider = logParserRepository.LogParserProviders.Single(p => p.UniqueName == profile.ParserUniqueName.Value);
            ILogParser parser = logParserProvider.CreateParser(logParserProvider.DeserializeConfiguration(profile.SerializedParserConfiguration.Value));

            // Build log source
            ILogSourceProvider logSourceProvider = logSourceRepository.LogSourceProviders.Single(p => p.UniqueName == result.LogSourceProviderName);
            ILogSource source = logSourceProvider.CreateLogSource(result.LogSourceConfiguration, parser);

            Title = WINDOW_TITLE + " - " + source.GetTitle();

            engine = engineFactory.CreateEngine(source, parser);

            engine.LoadingStatusChanged += HandleEngineLoadingStatusChanged;
            engine.ProcessingStatusChanged += HandleEngineProcessingStatusChanged;

            access.SetupListViews(engine.GetColumnInfos());
            engine.NotifySourceReady();

            LogEntries = engine.LogRecords;
            OnPropertyChanged(nameof(LogEntries));
            SearchResults = engine.SearchResults;
            OnPropertyChanged(nameof(SearchResults));

            enginePresentCondition.Value = true;
            bottomPaneVisible = false;

            if (!result.ProcessingProfileGuid.Equals(Guid.Empty))
            {
                ApplyProcessingProfile(result.ProcessingProfileGuid);
            }
        }

        private void WritelnLog(string message)
        {
            scriptLogDocument.Insert(scriptLogDocument.TextLength, $"{message}\n");
        }

        private void WriteLog(string message)
        {
            scriptLogDocument.Insert(scriptLogDocument.TextLength, message);
        }

        private void DoStopEngine()
        {
            engineStoppingCondition.Value = false;
            if (engine != null)
            {
                engine.LoadingStatusChanged -= HandleEngineLoadingStatusChanged;
                engine.ProcessingStatusChanged -= HandleEngineProcessingStatusChanged;
            }

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

        private void InternalOpen(OpenResult result)
        {
            if (engine != null)
            {
                engineStoppingCondition.Value = true;
                engine.Stop(() => EngineStoppedWithOpenCallback(result));
            }
            else
            {
                DoCreateEngine(result);
            }
        }

        private void DoOpen(List<string> droppedFiles = null)
        {
            var result = dialogService.OpenLog(new OpenFilesModel { DroppedFiles = droppedFiles });

            if (result.DialogResult)
            {
                InternalOpen(result.Result);
            }
        }

        private void DoOpenFromClipboard()
        {
            var result = dialogService.OpenLog(new OpenClipboardModel());

            if (result.DialogResult)
            {
                InternalOpen(result.Result);
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
                BottomPaneSelectedTabIndex = 0;
            }
        }

        private void DoCloseBootomPane()
        {
            BottomPaneVisible = false;
        }

        private void DoCloseRightPane()
        {
            RightPaneVisible = false;
        }

        private void CopyToClipboard(System.Collections.IList items)
        {
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

        private void DoCopy()
        {
            System.Collections.IList items = access.GetMainSelectedItems();
            CopyToClipboard(items);
        }

        private void DoCopySearchResults()
        {
            System.Collections.IList items = access.GetSearchSelectedItems();
            CopyToClipboard(items);
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

        private void DoVisualizeMessage()
        {
            var html = textParser.ParseToHtmlPage(selectedLogEntry.LogEntry.Message);
            dialogService.VisualizeMessage(new LogMessageVisualizerModel { Html = html });
        }

        private void DoExportToHtml()
        {
            IList<LogRecord> recordsToExport = GetItemsToExport();
            if (recordsToExport == null)
                return;

            string filename = winApiService.SaveFile(LogAnalyzer.Models.Constants.File.HtmlExportFilterDefinitions);
            if (filename != null)
            {
                string exported = exportService.ExportToHtml(recordsToExport, engine.GetColumnInfos());

                System.IO.File.WriteAllText(filename, exported);
            }
        }

        private IList<LogRecord> GetItemsToExport()
        {
            System.Collections.IList items = access.GetMainSelectedItems();

            IList<LogRecord> recordsToExport;

            if (items.Count == 0)
            {
                if (!messagingService.Ask("No entries selected - you will export all visible items. Are you sure?"))
                    recordsToExport = null;
                else
                    recordsToExport = engine.LogRecords;
            }
            else
            {
                recordsToExport = items
                    .Cast<LogRecord>()
                    .ToList();
            }

            return recordsToExport;
        }

        private void DoExportToStyledHtml()
        {
            IList<LogRecord> recordsToExport = GetItemsToExport();
            if (recordsToExport == null)
                return;

            string filename = winApiService.SaveFile(LogAnalyzer.Models.Constants.File.HtmlExportFilterDefinitions);
            if (filename != null)
            {
                string exported = exportService.ExportToStyledHtml(recordsToExport, engine.GetColumnInfos());

                System.IO.File.WriteAllText(filename, exported);
            }
        }

        private void DoCloseQuickSearch()
        {
            SearchBoxVisible = false;
        }

        private void DoQuickSearchDown()
        {
            LogRecord result = null;
            LogRecord searchFrom = selectedLogEntry;

            while (result == null)
            {
                result = engine.QuickSearch(searchString, searchFrom, true, searchCaseSensitive, searchWholeWords, searchRegex);

                if (result == null)
                {
                    if (messagingService.Ask("No entries found. Do you want to continue search from beginning?"))
                    {
                        searchFrom = null;
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    access.NavigateTo(result);
                }
            }
        }

        private void DoQuickSearchUp()
        {
            LogRecord result = null;
            LogRecord searchFrom = selectedLogEntry;

            while (result == null)
            {
                result = engine.QuickSearch(searchString, searchFrom, false, searchCaseSensitive, searchWholeWords, searchRegex);

                if (result == null)
                {
                    if (messagingService.Ask("No entries found. Do you want to continue search from end?"))
                    {
                        searchFrom = null;
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    access.NavigateTo(result);
                }
            }
        }

        private void DoShowQuickSearch()
        {
            SearchBoxVisible = true;
            access.FocusQuickSearchBox();
        }

        private void DoOpenConfiguration()
        {
            dialogService.OpenConfiguration();
        }

        private void DoOpenPythonEditor()
        {
            dialogService.OpenPythonEditor(this);
        }

        private void DoSaveProfile()
        {
            ProcessingProfileNameModel model = new ProcessingProfileNameModel { Name = "New profile" };
            var result = dialogService.ChooseProfileName(model);
            if (result.DialogResult)
            {
                var serializerSettings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects
                };
                string serializedHighlightSettings = JsonConvert.SerializeObject(engine.HighlightConfig, serializerSettings);
                string serializedFilterSettings = JsonConvert.SerializeObject(engine.FilterConfig, serializerSettings);

                ProcessingProfile processingProfile = new ProcessingProfile();
                processingProfile.Name.Value = result.Result.Name;
                processingProfile.Guid.Value = Guid.NewGuid();
                processingProfile.HighlightingSettings.Value = serializedHighlightSettings;
                processingProfile.FilteringSettings.Value = serializedFilterSettings;

                configurationService.Configuration.ProcessingProfiles.Add(processingProfile);

                eventBusService.Send(new ProcessingProfileListChanged());
            }
        }

        private int FindSelectedProfile()
        {
            var profiles = configurationService.Configuration.ProcessingProfiles;

            int index = 0;
            while (index < profiles.Count && !profiles[index].Guid.Value.Equals(selectedProcessingProfile.Guid))
                index++;

            if (index >= profiles.Count)
                throw new InvalidOperationException("Invalid profile!");
            return index;
        }

        private void DoDeleteProfile()
        {
            if (messagingService.Ask("Are you sure you want to delete selected processing profile?"))
            {
                int index = 0;
                SimpleCollection<ProcessingProfile> profiles = configurationService.Configuration.ProcessingProfiles;
                index = FindSelectedProfile();

                profiles.RemoveAt(index);

                eventBusService.Send(new ProcessingProfileListChanged());
            }
        }

        private void DoManageProfiles()
        {
            RightPaneVisible = true;
            RightPaneSelectedTabIndex = 0;
        }

        private void DoMoveProfileDown()
        {
            SimpleCollection<ProcessingProfile> profiles = configurationService.Configuration.ProcessingProfiles;

            int index = FindSelectedProfile();
            if (index == profiles.Count - 1)
                throw new InvalidOperationException("Cant move bottommost item down!");

            profiles.Move(index, index + 1);
            eventBusService.Send(new ProcessingProfileListChanged());
            SelectedProcessingProfileIndex = index + 1;
        }

        private void DoMoveProfileUp()
        {
            SimpleCollection<ProcessingProfile> profiles = configurationService.Configuration.ProcessingProfiles;

            int index = FindSelectedProfile();
            if (index == 0)
                throw new InvalidOperationException("Cant move topmost item up!");

            profiles.Move(index, index - 1);
            eventBusService.Send(new ProcessingProfileListChanged());
            SelectedProcessingProfileIndex = index - 1;
        }

        private int FindSelectedScript()
        {
            var scripts = configurationService.Configuration.StoredScripts;

            int index = 0;
            while (index < scripts.Count && !scripts[index].Guid.Value.Equals(selectedStoredScript.Guid))
                index++;

            if (index >= scripts.Count)
                throw new InvalidOperationException("Invalid script!");
            return index;
        }

        private void DoDeleteScript()
        {
            if (messagingService.Ask("Are you sure you want to delete selected script?"))
            {
                int index = 0;
                SimpleCollection<StoredScript> scripts = configurationService.Configuration.StoredScripts;
                index = FindSelectedScript();

                scripts.RemoveAt(index);

                eventBusService.Send(new StoredScriptListChanged());
            }
        }

        private void DoMoveScriptDown()
        {
            SimpleCollection<StoredScript> scripts = configurationService.Configuration.StoredScripts;

            int index = FindSelectedScript();
            if (index == scripts.Count - 1)
                throw new InvalidOperationException("Cant move bottommost item down!");

            scripts.Move(index, index + 1);
            eventBusService.Send(new StoredScriptListChanged());
            SelectedStoredScriptIndex = index + 1;

        }

        private void DoMoveScriptUp()
        {
            SimpleCollection<StoredScript> scripts = configurationService.Configuration.StoredScripts;

            int index = FindSelectedScript();
            if (index == 0)
                throw new InvalidOperationException("Cant move topmost item up!");

            scripts.Move(index, index - 1);
            eventBusService.Send(new StoredScriptListChanged());
            SelectedStoredScriptIndex = index - 1;
        }

        private void DoChooseProcessingProfile(object profile)
        {
            if (profile is ProcessingProfileViewModel processingProfileViewModel)
            {
                ApplyProcessingProfile(processingProfileViewModel.Guid);
            }
        }

        private void ApplyProcessingProfile(Guid processingProfileGuid)
        {
            var processingProfile = configurationService.Configuration.ProcessingProfiles
                                .Single(pp => pp.Guid.Value.Equals(processingProfileGuid));

            var serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            };

            HighlightConfig highlightConfig = JsonConvert.DeserializeObject<HighlightConfig>(processingProfile.HighlightingSettings.Value, serializerSettings);
            FilterConfig filterConfig = JsonConvert.DeserializeObject<FilterConfig>(processingProfile.FilteringSettings.Value, serializerSettings);
            engine.SetProcessingProfile(filterConfig, highlightConfig);
        }

        private void DoRunScript(object script)
        {
            if (script is StoredScriptViewModel)
            {
                RunChosenScript(script as StoredScriptViewModel);
            }            
        }

        private void RunChosenScript(StoredScriptViewModel storedScriptViewModel)
        {
            var script = configurationService.Configuration.StoredScripts
                .Single(s => s.Guid.Value.Equals(storedScriptViewModel.Guid));

            try
            {
                string scriptSource = File.ReadAllText(script.Filename.Value);
                ExecuteScript(scriptSource);
            }
            catch
            {
                messagingService.Stop("Cannot load script!");
            }
        }

        private void ExecuteScript(string source)
        {
            scriptLogDocument.Text = "";

            string path = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath);
            if (!path.EndsWith("\\"))
                path += "\\";
            path += "Lib\\";

            var scriptEngine = Python.CreateEngine();
            scriptEngine.SetSearchPaths(new List<string> { path });
            var scriptScope = scriptEngine.CreateScope();

            using (var logAnalyzer = new LogAnalyzerImpl(this.engine, WriteLog, WritelnLog))
            {
                scriptScope.SetVariable("LogAnalyzer", logAnalyzer);

                var errorListener = new PythonScriptErrorListener((ScriptSource erroneousScriptSource, string message, SourceSpan span, int errorCode, Severity severity) =>
                {
                    WritelnLog($"{severity.ToString()} {errorCode} at {span.Start.ToString()}: {message}\n");
                });

                var scriptSource = scriptEngine.CreateScriptSourceFromString(source);
                var compiled = scriptSource.Compile(errorListener);

                if (compiled == null)
                {
                    messagingService.Warn("There were compilation errors. Look at the script log for details.");
                    BottomPaneVisible = true;
                    BottomPaneSelectedTabIndex = 1;
                    return;
                }

                try
                {
                    compiled.Execute(scriptScope);

                    // Display log
                    BottomPaneVisible = true;
                    BottomPaneSelectedTabIndex = 1;
                }
                catch (Exception e)
                {
                    scriptLogDocument.Insert(scriptLogDocument.TextLength, $"Script runtime error: {e.Message}");
                }
            }
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // IScriptingHost implementation --------------------------------------

        void IScriptingHost.Run(string script)
        {
            ExecuteScript(script);
        }

        BaseCondition IScriptingHost.CanRunCondition => scriptRunCondition;

        // IEventListener<ProcessingProfileListChanged> implementation --------

        void IEventListener<ProcessingProfileListChanged>.Receive(ProcessingProfileListChanged @event)
        {
            BuildProcessingProfiles();
        }

        // IEventListener<StoredScriptListChanged> implementation -------------

        void IEventListener<StoredScriptListChanged>.Receive(StoredScriptListChanged @event)
        {
            BuildStoredScripts();
        }

        // Public methods -----------------------------------------------------

        public MainWindowViewModel(IMainWindowAccess access,
            IDialogService dialogService,
            IEngineFactory engineFactory,
            ILogParserRepository logParserRepository,
            ILogSourceRepository logSourceRepository,
            IConfigurationService configurationService,
            ITextParser textParser,
            IMessagingService messagingService,
            IWinApiService winApiService,
            IExportService exportService,
            IEventBusService eventBusService)
        {
            this.access = access;
            this.dialogService = dialogService;
            this.engineFactory = engineFactory;
            this.logParserRepository = logParserRepository;
            this.logSourceRepository = logSourceRepository;
            this.configurationService = configurationService;
            this.textParser = textParser;
            this.messagingService = messagingService;
            this.winApiService = winApiService;
            this.exportService = exportService;
            this.eventBusService = eventBusService;

            this.eventBusService.Register<ProcessingProfileListChanged>(this);
            this.eventBusService.Register<StoredScriptListChanged>(this);

            engineStoppingCondition = new SimpleCondition(false);
            generalCommandCondition = !engineStoppingCondition;
            enginePresentCondition = new SimpleCondition(false);
            generalEnginePresentCondition = enginePresentCondition & !engineStoppingCondition;
            itemSelectedCondition = new SimpleCondition(false);
            searchStringExists = new SimpleCondition(false);
            firstProfileSelectedCondition = new SimpleCondition(false);
            lastProfileSelectedCondition = new SimpleCondition(false);
            profileSelectedCondition = new SimpleCondition(false);
            firstScriptSelectedCondition = new SimpleCondition(false);
            lastScriptSelectedCondition = new SimpleCondition(false);
            scriptSelectedCondition = new SimpleCondition(false);
            scriptRunCondition = generalEnginePresentCondition;

            loadingStatusText = "Loading...";
            processingStatusText = "Processing...";
            loadingStatus = false;
            processingStatus = false;

            scriptLogDocument = new TextDocument();

            processingProfiles = new ObservableCollection<ProcessingProfileViewModel>();
            processingProfileClickCommand = new AppCommand((obj) => DoChooseProcessingProfile(obj), generalEnginePresentCondition);

            BuildProcessingProfiles();

            storedScripts = new ObservableCollection<StoredScriptViewModel>();
            storedScriptClickCommand = new AppCommand((obj) => DoRunScript(obj), generalEnginePresentCondition);

            BuildStoredScripts();

            OpenCommand = new AppCommand((obj) => DoOpen(), generalCommandCondition);
            HighlightConfigCommand = new AppCommand((obj) => DoHighlightConfig(), generalEnginePresentCondition);
            FilterConfigCommand = new AppCommand((obj) => DoFilterConfig(), generalEnginePresentCondition);
            SearchCommand = new AppCommand((obj) => DoSearch(), generalEnginePresentCondition);
            CloseBottomPaneCommand = new AppCommand((obj) => DoCloseBootomPane());
            CloseRightPaneCommand = new AppCommand((obj) => DoCloseRightPane());
            CopyCommand = new AppCommand((obj) => DoCopy(), generalEnginePresentCondition);
            JumpToTimeCommand = new AppCommand((obj) => DoJumpToTime(), generalEnginePresentCondition);
            SetBookmarkCommand = new AppCommand((obj) => DoSetBookmark((string)obj), generalEnginePresentCondition & itemSelectedCondition);
            GotoBookmarkCommand = new AppCommand((obj) => DoGotoBookmark((string)obj), generalEnginePresentCondition);
            AddHighlightingRuleCommand = new AppCommand((obj) => DoAddHighlightingRule(), generalEnginePresentCondition & itemSelectedCondition);
            AddFilteringRuleCommand = new AppCommand((obj) => DoAddFilteringRule(), generalEnginePresentCondition & itemSelectedCondition);
            ToggleProfilingPointCommand = new AppCommand((obj) => DoToggleProfilingPointCommand(), generalEnginePresentCondition & itemSelectedCondition);
            ClearProfilingPointsCommand = new AppCommand((obj) => DoClearProfilingPointsCommand(), generalEnginePresentCondition);
            AnnotateCommand = new AppCommand((obj) => DoAnnotate(), generalEnginePresentCondition & itemSelectedCondition);
            VisualizeMessageCommand = new AppCommand((obj) => DoVisualizeMessage(), generalEnginePresentCondition & itemSelectedCondition);
            ExportToHtmlCommand = new AppCommand((obj) => DoExportToHtml(), generalEnginePresentCondition);
            ExportToStyledHtmlCommand = new AppCommand((obj) => DoExportToStyledHtml(), generalEnginePresentCondition);
            QuickSearchUpCommand = new AppCommand((obj) => DoQuickSearchUp(), generalEnginePresentCondition & searchStringExists);
            QuickSearchDownCommand = new AppCommand((obj) => DoQuickSearchDown(), generalEnginePresentCondition & searchStringExists);
            CloseQuickSearchCommand = new AppCommand((obj) => DoCloseQuickSearch());
            ShowQuickSearchCommand = new AppCommand((obj) => DoShowQuickSearch());
            OpenFromClipboardCommand = new AppCommand((obj) => DoOpenFromClipboard());
            ConfigurationCommand = new AppCommand((obj) => DoOpenConfiguration());
            OpenPythonEditorCommand = new AppCommand((obj) => DoOpenPythonEditor());
            SaveProfileCommand = new AppCommand((obj) => DoSaveProfile(), generalEnginePresentCondition);
            MoveProfileUpCommand = new AppCommand((obj) => DoMoveProfileUp(), profileSelectedCondition & (!firstProfileSelectedCondition));
            MoveProfileDownCommand = new AppCommand((obj) => DoMoveProfileDown(), profileSelectedCondition & (!lastProfileSelectedCondition));
            DeleteProfileCommand = new AppCommand((obj) => DoDeleteProfile(), profileSelectedCondition);
            MoveScriptUpCommand = new AppCommand((obj) => DoMoveScriptUp(), scriptSelectedCondition & (!firstScriptSelectedCondition));
            MoveScriptDownCommand = new AppCommand((obj) => DoMoveScriptDown(), scriptSelectedCondition & (!lastScriptSelectedCondition));
            DeleteScriptCommand = new AppCommand((obj) => DoDeleteScript(), scriptSelectedCondition);
            ManageProfilesCommand = new AppCommand((obj) => DoManageProfiles());
            CopySearchResultsCommand = new AppCommand((obj) => DoCopySearchResults(), generalEnginePresentCondition);
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

        public ICommand CloseBottomPaneCommand { get; }

        public ICommand CloseRightPaneCommand { get; }

        public ICommand CopyCommand { get; }

        public ICommand JumpToTimeCommand { get; }

        public ICommand SetBookmarkCommand { get; }

        public ICommand GotoBookmarkCommand { get; }

        public ICommand AddHighlightingRuleCommand { get; }

        public ICommand AddFilteringRuleCommand { get; }

        public ICommand ToggleProfilingPointCommand { get; }

        public ICommand ClearProfilingPointsCommand { get; }

        public ICommand AnnotateCommand { get; }

        public ICommand VisualizeMessageCommand { get; }

        public ICommand ExportToHtmlCommand { get; }

        public ICommand ExportToStyledHtmlCommand { get; }

        public ICommand QuickSearchUpCommand { get; }

        public ICommand QuickSearchDownCommand { get; }

        public ICommand CloseQuickSearchCommand { get; }

        public ICommand ShowQuickSearchCommand { get; }

        public ICommand OpenFromClipboardCommand { get; }

        public ICommand ConfigurationCommand { get; }

        public ICommand OpenPythonEditorCommand { get; }

        public ICommand LicenseCommand { get; }

        public ICommand SaveProfileCommand { get; }

        public ICommand MoveProfileUpCommand { get; }

        public ICommand MoveProfileDownCommand { get; }

        public ICommand ManageProfilesCommand { get; }

        public ICommand DeleteProfileCommand { get; }

        public ICommand MoveScriptUpCommand { get; }

        public ICommand MoveScriptDownCommand { get; }

        public ICommand DeleteScriptCommand { get; }

        public ICommand CopySearchResultsCommand { get; }

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

        public int BottomPaneSelectedTabIndex
        {
            get => bottomPaneSelectedTabIndex;
            set
            {
                bottomPaneSelectedTabIndex = value;
                OnPropertyChanged(nameof(BottomPaneSelectedTabIndex));
            }
        }

        public bool RightPaneVisible
        {
            get => rightPaneVisible;
            set
            {
                rightPaneVisible = value;
                OnPropertyChanged(nameof(RightPaneVisible));
            }
        }

        public int RightPaneSelectedTabIndex
        {
            get => rightPaneSelectedTabIndex;
            set
            {
                rightPaneSelectedTabIndex = value;
                OnPropertyChanged(nameof(RightPaneSelectedTabIndex));
            }
        }

        public string SearchString
        {
            get => searchString;
            set
            {
                searchString = value;
                searchStringExists.Value = !string.IsNullOrEmpty(value);
                OnPropertyChanged(nameof(SearchString));
            }
        }

        public bool SearchBoxVisible
        {
            get => searchBoxVisible;
            set
            {
                searchBoxVisible = value;
                OnPropertyChanged(nameof(SearchBoxVisible));
            }
        }

        public bool SearchCaseSensitive
        {
            get => searchCaseSensitive;
            set
            {
                searchCaseSensitive = value;
                OnPropertyChanged(nameof(SearchCaseSensitive));
            }
        }

        public bool SearchWholeWords
        {
            get => searchWholeWords;
            set
            {
                searchWholeWords = value;
                OnPropertyChanged(nameof(SearchWholeWords));
            }
        }

        public bool SearchRegex
        {
            get => searchRegex;
            set
            {
                searchRegex = value;
                OnPropertyChanged(nameof(SearchRegex));
            }
        }

        public TextDocument ScriptLogDocument => scriptLogDocument;

        public string LoadingStatusText
        {
            get => loadingStatusText;
            set
            {
                loadingStatusText = value;
                OnPropertyChanged(nameof(LoadingStatusText));
            }
        }

        public bool LoadingStatus
        {
            get => loadingStatus;
            set
            {
                loadingStatus = value;
                OnPropertyChanged(nameof(LoadingStatus));
            }
        }

        public string ProcessingStatusText
        {
            get => processingStatusText;
            set
            {
                processingStatusText = value;
                OnPropertyChanged(nameof(ProcessingStatusText));
            }
        }

        public bool ProcessingStatus
        {
            get => processingStatus;
            set
            {
                processingStatus = value;
                OnPropertyChanged(nameof(ProcessingStatus));
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

        public int SelectedProcessingProfileIndex
        {
            get => selectedProcessingProfileIndex;
            set
            {
                selectedProcessingProfileIndex = value;
                firstProfileSelectedCondition.Value = (value == 0);
                lastProfileSelectedCondition.Value = (value == processingProfiles.Count - 1);
                profileSelectedCondition.Value = (value >= 0);

                OnPropertyChanged(nameof(SelectedProcessingProfileIndex));
            }
        }

        public StoredScriptViewModel SelectedStoredScript
        {
            get => selectedStoredScript;
            set
            {
                selectedStoredScript = value;
                OnPropertyChanged(nameof(SelectedStoredScript));
            }
        }

        public int SelectedStoredScriptIndex
        {
            get => selectedStoredScriptIndex;
            set
            {
                selectedStoredScriptIndex = value;

                firstScriptSelectedCondition.Value = (value == 0);
                lastScriptSelectedCondition.Value = (value == storedScripts.Count - 1);
                scriptSelectedCondition.Value = (value >= 0);

                OnPropertyChanged(nameof(SelectedStoredScriptIndex));
            }
        }

        public ObservableCollection<StoredScriptViewModel> StoredScripts => storedScripts;

        public string Title
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
    }
}
