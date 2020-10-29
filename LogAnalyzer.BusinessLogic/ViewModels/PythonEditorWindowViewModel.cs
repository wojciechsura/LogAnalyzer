using ICSharpCode.AvalonEdit.Document;
using LogAnalyzer.BusinessLogic.ViewModels.ApiSamples;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using LogAnalyzer.BusinessLogic.ViewModels.PythonEditor;
using LogAnalyzer.Common.Tools;
using LogAnalyzer.Configuration;
using LogAnalyzer.Models.Events;
using LogAnalyzer.Models.Views.ScriptNameWindow;
using LogAnalyzer.Scripting;
using LogAnalyzer.Services.Interfaces;
using Spooksoft.VisualStateManager.Commands;
using Spooksoft.VisualStateManager.Conditions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LogAnalyzer.BusinessLogic.ViewModels
{
    public class PythonEditorWindowViewModel : INotifyPropertyChanged, IEventListener<StoredScriptListChanged>
    {
        // Private fields -----------------------------------------------------

        private readonly IPythonEditorWindowAccess access;
        private readonly IScriptingHost scriptingHost;
        private readonly IConfigurationService configurationService;
        private readonly IPathProviderService pathProviderService;
        private readonly IDialogService dialogService;
        private readonly IMessagingService messagingService;
        private readonly IEventBusService eventBusService;
        private readonly IScriptApiSampleRepository scriptApiSampleRepository;
        private readonly TextDocument document;

        private Guid currentScriptGuid;

        private readonly ICommand storedScriptClickCommand;
        private readonly ICommand apiSampleClickCommand;
        private readonly ObservableCollection<StoredScriptViewModel> storedScripts;
        private readonly ObservableCollection<ApiSampleViewModel> apiSamples;

        // Private methods ----------------------------------------------------

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

        private void BuildApiSamples()
        {
            apiSamples.Clear();
            this.scriptApiSampleRepository.ApiSamples
                .Select(s => new ApiSampleViewModel(s.Name, s.Id, apiSampleClickCommand))
                .ToList()
                .ForEach(s => apiSamples.Add(s));
        }

        private void InternalOpenScript(StoredScriptViewModel script)
        {
            StoredScript storedScript = configurationService.Configuration.StoredScripts.Single(ss => ss.Guid.Value.Equals(script.Guid));

            try
            {
                string scriptSource = File.ReadAllText(storedScript.Filename.Value);
                document.Text = scriptSource;
                currentScriptGuid = storedScript.Guid.Value;
            }
            catch
            {
                messagingService.Stop("Cannot open selected script!");
            }
        }

        private void InternalInsertApiSample(ApiSampleViewModel sample)
        {
            string resource = scriptApiSampleRepository.ApiSamples.Single(s => s.Id == sample.Id)
                .ResourceName;

            string source = ResourceReader.ReadEmbeddedResource(Assembly.GetExecutingAssembly(), resource);
            document.Insert(access.GetCaretOffset(), source);
        }

        private void SaveExistingScript(StoredScript storedScript)
        {                       
            File.WriteAllText(storedScript.Filename.Value, document.Text);
        }

        private void DoRun()
        {
            scriptingHost.Run(document.Text);
        }

        private void DoSaveAs()
        {
            var result = dialogService.ChooseScriptName(new ScriptNameModel { Name = "New script" });
            if (result.DialogResult)
            {
                try
                {
                    Guid scriptGuid = Guid.NewGuid();

                    StoredScript storedScript = new StoredScript();
                    storedScript.Name.Value = result.Result.Name;
                    storedScript.Guid.Value = scriptGuid;
                    storedScript.Filename.Value = Path.Combine(pathProviderService.GetScriptsPath(), storedScript.Guid.Value + ".py");

                    // Save script to file
                    SaveExistingScript(storedScript);

                    // Upon successful saving, add entry in configuration and
                    // keep script's guid as current.
                    configurationService.Configuration.StoredScripts.Add(storedScript);
                    eventBusService.Send(new StoredScriptListChanged());

                    currentScriptGuid = scriptGuid;
                }
                catch
                {
                    messagingService.Stop("Cannot save script!");
                }
            }
        }

        private void DoSave()
        {
            if (currentScriptGuid == Guid.Empty)
            {
                DoSaveAs();
                return;
            }

            StoredScript storedScript = configurationService.Configuration.StoredScripts.Single(ss => ss.Guid.Value.Equals(currentScriptGuid));
            SaveExistingScript(storedScript);
        }

        private void DoOpenScript(object obj)
        {
            if (!(obj is StoredScriptViewModel))
                throw new ArgumentException("Invalid parameter for DoOpenScript!");

            InternalOpenScript((StoredScriptViewModel)obj);
        }

        private void DoInsertApiSample(object obj)
        {
            if (!(obj is ApiSampleViewModel))
                throw new ArgumentException("Invalid parameter for DoInsertApiSampl!");

            InternalInsertApiSample((ApiSampleViewModel)obj);
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // IEventListener<StoredScriptListChanged> implementation ------------

        void IEventListener<StoredScriptListChanged>.Receive(StoredScriptListChanged @event)
        {
            BuildStoredScripts();
        }

        // Public methods -----------------------------------------------------

        public PythonEditorWindowViewModel(IPythonEditorWindowAccess access, 
            IScriptingHost scriptingHost, 
            IConfigurationService configurationService,
            IPathProviderService pathProviderService,
            IDialogService dialogService,
            IMessagingService messagingService,
            IEventBusService eventBusService,
            IScriptApiSampleRepository scriptApiSampleRepository)
        {
            this.access = access;
            this.scriptingHost = scriptingHost;
            this.configurationService = configurationService;
            this.pathProviderService = pathProviderService;
            this.dialogService = dialogService;
            this.messagingService = messagingService;
            this.eventBusService = eventBusService;
            this.scriptApiSampleRepository = scriptApiSampleRepository;

            this.eventBusService.Register<StoredScriptListChanged>(this);

            this.currentScriptGuid = Guid.Empty;
            this.document = new TextDocument();

            this.storedScripts = new ObservableCollection<StoredScriptViewModel>();
            this.apiSamples = new ObservableCollection<ApiSampleViewModel>();

            this.storedScriptClickCommand = new AppCommand((obj) => DoOpenScript(obj));
            this.apiSampleClickCommand = new AppCommand((obj) => DoInsertApiSample(obj));

            BuildStoredScripts();
            BuildApiSamples();
            
            this.RunCommand = new AppCommand((obj) => DoRun(), scriptingHost.CanRunCondition);
            this.SaveCommand = new AppCommand((obj) => DoSave());
            this.SaveAsCommand = new AppCommand((obj) => DoSaveAs());
        }

        // Public properties --------------------------------------------------

        public TextDocument Document => document;

        public ICommand RunCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand SaveAsCommand { get; }

        public ObservableCollection<StoredScriptViewModel> StoredScripts => storedScripts;

        public IReadOnlyList<ApiSampleViewModel> ApiSamples => apiSamples;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
