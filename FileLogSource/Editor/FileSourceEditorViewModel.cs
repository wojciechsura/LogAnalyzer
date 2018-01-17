using LogAnalyzer.API.LogSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileLogSource.Editor
{
    public class FileSourceEditorViewModel : ILogSourceEditorViewModel
    {
        private const string DISPLAY_NAME = "Single file";
        private const string EDITOR_RESOURCE = "FileLogEditorDataTemplate";

        public ILogSourceConfiguration BuildConfiguration()
        {
            return new FileLogSourceConfiguration();
        }


        public string DisplayName => DISPLAY_NAME;

        public string EditorResource => EDITOR_RESOURCE;
    }
}
