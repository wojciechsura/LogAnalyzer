using LogAnalyzer.API.LogSource;
using LogAnalyzer.Wpf.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileLogSource.Editor
{
    public class FileSourceEditorViewModel : ILogSourceEditorViewModel
    {
        // Private constants --------------------------------------------------

        private const string DISPLAY_NAME = "Single file";
        private const string EDITOR_RESOURCE = "FileLogEditorDataTemplate";

        // ILogSourceEditorVoewModel implementation ---------------------------

        public ILogSourceConfiguration BuildConfiguration()
        {
            return new FileLogSourceConfiguration();
        }

        // Private methods ----------------------------------------------------

        private void DoOpenFile()
        {
            // TODO
        }

        // Public methods -----------------------------------------------------

        public FileSourceEditorViewModel()
        {
            OpenFileCommand = new SimpleCommand((obj) => DoOpenFile());
        }

        // Public properties --------------------------------------------------

        public string DisplayName => DISPLAY_NAME;

        public string EditorResource => EDITOR_RESOURCE;

        public ICommand OpenFileCommand { get; private set; }
    }
}
