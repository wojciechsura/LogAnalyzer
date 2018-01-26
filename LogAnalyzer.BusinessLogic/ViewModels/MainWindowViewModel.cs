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
using LogAnalyzer.Models.Engine;

namespace LogAnalyzer.BusinessLogic.ViewModels
{
    public class MainWindowViewModel
    {
        // Private fields -----------------------------------------------------

        private readonly IMainWindowAccess access;
        private readonly IDialogService dialogService;
        private readonly ILogSourceRepository logSourceRepository;
        private readonly ILogParserRepository logParserRepository;
        private readonly IConfigurationService configurationService;
        private readonly IEngineFactory engineFactory;

        private IEngine engine;

        // Private methods ----------------------------------------------------

        private void DoOpen()
        {
            var result = dialogService.OpenLog();

            if (result.DialogResult)
            {
                #warning Close current engine

                // TODO finish

                // Build log parser
                LogParserProfile profile = configurationService.Configuration.LogParserProfiles.Single(p => p.Guid.Equals(result.Result.ParserProfileGuid));
                ILogParserProvider logParserProvider = logParserRepository.LogParserProviders.Single(p => p.UniqueName == profile.ParserUniqueName.Value);
                ILogParser parser = logParserProvider.CreateParser(logParserProvider.DeserializeConfiguration(profile.SerializedParserConfiguration.Value));

                // Build log source
                ILogSourceProvider logSourceProvider = logSourceRepository.LogSourceProviders.Single(p => p.UniqueName == result.Result.LogSourceProviderName);
                ILogSource source = logSourceProvider.CreateLogSource(result.Result.LogSourceConfiguration, parser);

                BaseDocument document = new BaseDocument();

                engine = engineFactory.CreateEngine(source, parser, document);
                engine.NotifySourceReady();
            }
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

            OpenCommand = new SimpleCommand((obj) => DoOpen());
        }

        // Public properties --------------------------------------------------

        public ICommand OpenCommand { get; private set; }
    }
}
