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
        private Condition enginePresentCondition;
        private BaseCondition generalEnginePresentCondition;

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
            access.SetupListView(engine.GetColumnInfos());
            engine.NotifySourceReady();

            LogEntries = engine.LogEntries;
            OnPropertyChanged(nameof(LogEntries));

            enginePresentCondition.Value = true;
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

            FindModel model = new FindModel(availableCustomColumns);
            var result = dialogService.OpenFind(model);
            if (result.DialogResult)
            {
                // TODO
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
            enginePresentCondition = new Condition(false);
            generalEnginePresentCondition = enginePresentCondition & !engineStoppingCondition;

            OpenCommand = new SimpleCommand((obj) => DoOpen(), generalCommandCondition);
            HighlightConfigCommand = new SimpleCommand((obj) => DoHighlightConfig(), generalEnginePresentCondition);
            FilterConfigCommand = new SimpleCommand((obj) => DoFilterConfig(), generalEnginePresentCondition);
            SearchCommand = new SimpleCommand((obj) => DoSearch(), generalEnginePresentCondition);
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

        // Public properties --------------------------------------------------

        public ICommand OpenCommand { get; }

        public ICommand HighlightConfigCommand { get; }

        public ICommand FilterConfigCommand { get; }

        public ICommand SearchCommand { get; }

        public ObservableRangeCollection<HighlightedLogEntry> LogEntries { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
