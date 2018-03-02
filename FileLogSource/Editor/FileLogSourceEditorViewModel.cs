using LogAnalyzer.API.LogSource;
using LogAnalyzer.API.Types;
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
    public class FileLogSourceEditorViewModel : ILogSourceEditorViewModel, INotifyPropertyChanged
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
            string filename = winApiService.OpenFile(LogAnalyzer.Models.Constants.File.LogFilterDefinitions);
            if (filename != null)
            {
                this.filename = filename;
                OnPropertyChanged(nameof(Filename));
                OnSourceChanged();
            }
        }

        private void OnSourceChanged()
        {
            SourceChanged?.Invoke(this, new EventArgs());
        }

        private void LoadConfig(FileLogSourceConfiguration config)
        {
            Filename = config.Filename;
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ValidationResult Validate()
        {
            if (String.IsNullOrEmpty(filename))
                return new ValidationResult(false, "Choose log file");
            if (!File.Exists(filename))
                return new ValidationResult(false, "File does not exist!");

            return new ValidationResult(true, null);
        }

        // Public methods -----------------------------------------------------

        public FileLogSourceEditorViewModel(ILogSourceProvider provider, IWinApiService winApiService)
        {
            this.Provider = provider;
            this.winApiService = winApiService;

            OpenFileCommand = new SimpleCommand((obj) => DoOpenFile());
        }

        public void LoadConfiguration(ILogSourceConfiguration configuration)
        {
            if (configuration is FileLogSourceConfiguration config)
                LoadConfig(config);
            else
                throw new ArgumentException("Invalid configuration!");
        }

        public List<string> ProvideSampleLines()
        {
            try
            {
                List<string> result = new List<string>();

                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                using (TextReader reader = new StreamReader(fs))
                {
                    int i = 0;
                    string line;
                    do
                    {
                        line = reader.ReadLine();
                        if (line != null)
                        {
                            result.Add(line);
                            i++;
                        }
                    }
                    while (i < 10 && line != null);
                }

                return result;
            }
            catch
            {
                return new List<string>();
            }
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

        public ILogSourceProvider Provider { get; }

        public bool ProvidesSampleLines => true;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler SourceChanged;
    }
}
