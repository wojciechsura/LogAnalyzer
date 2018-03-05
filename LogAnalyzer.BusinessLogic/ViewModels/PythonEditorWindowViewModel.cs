using ICSharpCode.AvalonEdit.Document;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using LogAnalyzer.Scripting;
using LogAnalyzer.Wpf.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LogAnalyzer.BusinessLogic.ViewModels
{
    public class PythonEditorWindowViewModel : INotifyPropertyChanged
    {
        private readonly IPythonEditorWindowAccess access;
        private readonly IScriptingHost scriptingHost;
        private readonly TextDocument document;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void DoRun()
        {
            scriptingHost.Run(document.Text);
        }

        public PythonEditorWindowViewModel(IPythonEditorWindowAccess access, IScriptingHost scriptingHost)
        {
            this.access = access;
            this.scriptingHost = scriptingHost;

            this.document = new TextDocument();

            this.RunCommand = new SimpleCommand((obj) => DoRun());
        }

        public TextDocument Document => document;

        public ICommand RunCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
