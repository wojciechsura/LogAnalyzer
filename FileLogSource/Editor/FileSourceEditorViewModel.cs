using LogAnalyzer.API.LogSource;
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

namespace FileLogSource.Editor
{
    public class FileSourceEditorViewModel : ILogSourceEditorViewModel, INotifyPropertyChanged
    {
        // Private constants --------------------------------------------------

        private const string DISPLAY_NAME = "Single file";
        private const string EDITOR_RESOURCE = "FileLogEditorDataTemplate";

        // Private fields -----------------------------------------------------

        private IWinApiService winApiService;
        private string filename;

        // ILogSourceEditorVoewModel implementation ---------------------------

        public ILogSourceConfiguration BuildConfiguration()
        {
            return new FileLogSourceConfiguration(filename);
        }

        // Private methods ----------------------------------------------------

        private void DoOpenFile()
        {
            string filename = winApiService.OpenFile(LogAnalyzer.Common.Constants.File.LogFilterDefinitions);
            if (filename != null) {
                this.filename = filename;
                OnPropertyChanged(nameof(Filename));
            }
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public bool Validate()
        {            
            return filename != null && File.Exists(filename);
        }

        // Public methods -----------------------------------------------------

        public FileSourceEditorViewModel(IWinApiService winApiService)
        {
            this.winApiService = winApiService;

            OpenFileCommand = new SimpleCommand((obj) => DoOpenFile());
        }

        // Public properties --------------------------------------------------

        public string DisplayName => DISPLAY_NAME;

        public string EditorResource => EDITOR_RESOURCE;

        public ICommand OpenFileCommand { get; private set; }

        public string Filename
        {
            get
            {
                return filename;
            }
            set
            {
                filename = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
