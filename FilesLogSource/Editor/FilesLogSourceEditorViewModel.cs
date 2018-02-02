using LogAnalyzer.API.LogSource;
using LogAnalyzer.API.Types;
using LogAnalyzer.Services.Interfaces;
using LogAnalyzer.Wpf.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FilesLogSource.Editor
{
    class FilesLogSourceEditorViewModel : ILogSourceEditorViewModel, INotifyPropertyChanged
    {
        // Public classes -----------------------------------------------------

        public class FileInfo
        {
            public string Filename { get; set; }
        }

        // Private constants --------------------------------------------------

        private const string DISPLAY_NAME = "Multiple files";
        private const string EDITOR_RESOURCE = "FilesLogEditorDataTemplate";

        // Private fields -----------------------------------------------------

        private IWinApiService winApiService;
        private ILogSourceProvider provider;
        private ObservableCollection<FileInfo> files;
        private FileInfo selectedFile;
        private int selectedFileIndex;
        private bool autoSort;

        private Condition fileSelectedCondition;
        private Condition firstFileSelectedCondition;
        private Condition lastFileSelectedCondition;

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Public methods -----------------------------------------------------

        public FilesLogSourceEditorViewModel(ILogSourceProvider provider, IWinApiService winApiService)
        {
            this.winApiService = winApiService;
            this.provider = provider;
            files = new ObservableCollection<FileInfo>();
            selectedFile = null;
            autoSort = true;

            fileSelectedCondition = new Condition(false);
            firstFileSelectedCondition = new Condition(false);
            lastFileSelectedCondition = new Condition(false);

            AddFilesCommand = new SimpleCommand((obj) => DoAddFiles());
            RemoveFileCommand = new SimpleCommand((obj) => DoRemoveFile(), fileSelectedCondition);
            MoveFileUpCommand = new SimpleCommand((obj) => DoMoveFileUp(), fileSelectedCondition & !firstFileSelectedCondition);
            MoveFileDownCommand = new SimpleCommand((obj) => DoMoveFileDown(), fileSelectedCondition & !lastFileSelectedCondition);
        }

        private void DoMoveFileDown()
        {
            files.Move(selectedFileIndex, selectedFileIndex + 1);
        }

        private void DoMoveFileUp()
        {
            files.Move(selectedFileIndex, selectedFileIndex - 1);
        }

        private void DoRemoveFile()
        {
            files.RemoveAt(selectedFileIndex);
        }

        private void DoAddFiles()
        {
            List<string> newFiles = winApiService.OpenFiles(LogAnalyzer.Models.Constants.File.LogFilterDefinitions);
            if (newFiles != null)
            {
                foreach (var file in newFiles)
                    files.Add(new FileInfo { Filename = file });
            }
        }

        public ILogSourceConfiguration BuildConfiguration()
        {
            FilesLogSourceConfiguration configuration = new FilesLogSourceConfiguration
            {
                Files = files.Select(f => f.Filename).ToList(),
                AutoSort = autoSort
            };

            return configuration;
        }

        public ValidationResult Validate()
        {
            if (files.Count == 0)
            {
                return new ValidationResult(false, "No files were selected.");
            }

            for (int i = 0; i < files.Count; i++)
            {
                if (!File.Exists(files[i].Filename))
                {
                    return new ValidationResult(false, $"File {files[i].Filename} does not exist!");
                }
            }

            return new ValidationResult(true, null);
        }

        // Public properties --------------------------------------------------

        public string DisplayName => DISPLAY_NAME;

        public string EditorResource => EDITOR_RESOURCE;

        public ILogSourceProvider Provider => provider;

        public ObservableCollection<FileInfo> Files => files;

        public FileInfo SelectedFile
        {
            get => selectedFile;
            set
            {
                selectedFile = value;
                OnPropertyChanged(nameof(SelectedFile));
            }
        }

        public int SelectedFileIndex
        {
            set
            {
                fileSelectedCondition.Value = value >= 0;
                firstFileSelectedCondition.Value = value == 0;
                lastFileSelectedCondition.Value = value == files.Count - 1;
                selectedFileIndex = value;
            }
        }

        public bool AutoSort
        {
            get => autoSort;
            set
            {
                autoSort = value;
                OnPropertyChanged(nameof(AutoSort));
            }
        }

        public ICommand AddFilesCommand { get; }
        public ICommand RemoveFileCommand { get; }
        public ICommand MoveFileUpCommand { get; }
        public ICommand MoveFileDownCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
