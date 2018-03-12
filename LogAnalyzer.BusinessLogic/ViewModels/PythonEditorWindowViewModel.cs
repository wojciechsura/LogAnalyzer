using ICSharpCode.AvalonEdit.Document;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using LogAnalyzer.Configuration;
using LogAnalyzer.Models.Views.ScriptNameWindow;
using LogAnalyzer.Scripting;
using LogAnalyzer.Services.Interfaces;
using LogAnalyzer.Wpf.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LogAnalyzer.BusinessLogic.ViewModels
{
    public class PythonEditorWindowViewModel : INotifyPropertyChanged
    {
        // Private fields -----------------------------------------------------

        private readonly IPythonEditorWindowAccess access;
        private readonly IScriptingHost scriptingHost;
        private readonly IConfigurationService configurationService;
        private readonly IPathProviderService pathProviderService;
        private readonly IDialogService dialogService;
        private readonly IMessagingService messagingService;
        private readonly TextDocument document;

        private Guid currentScriptGuid;

        // Private methods ----------------------------------------------------

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

            StoredScript storedScript = configurationService.Configuration.StoredScripts.Single(ss => ss.Guid.Equals(currentScriptGuid));
            SaveExistingScript(storedScript);
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Public methods -----------------------------------------------------

        public PythonEditorWindowViewModel(IPythonEditorWindowAccess access, 
            IScriptingHost scriptingHost, 
            IConfigurationService configurationService,
            IPathProviderService pathProviderService,
            IDialogService dialogService,
            IMessagingService messagingService)
        {
            this.access = access;
            this.scriptingHost = scriptingHost;
            this.configurationService = configurationService;
            this.pathProviderService = pathProviderService;
            this.dialogService = dialogService;
            this.messagingService = messagingService;

            this.currentScriptGuid = Guid.Empty;
            this.document = new TextDocument();

            this.RunCommand = new SimpleCommand((obj) => DoRun());
            this.SaveCommand = new SimpleCommand((obj) => DoSave());
            this.SaveAsCommand = new SimpleCommand((obj) => DoSaveAs());
        }

        // Public properties --------------------------------------------------

        public TextDocument Document => document;

        public ICommand RunCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand SaveAsCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
