using FileLogSource.Editor;
using LogAnalyzer.API.LogSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace FileLogSource
{
    public class FileLogSourceProvider : ILogSourceProvider
    {
        private readonly string UNIQUE_NAME = "LogSource.File";

        public ILogSourceEditorViewModel CreateEditorViewModel()
        {
            return LogAnalyzer.Dependencies.Container.Instance.Resolve<FileSourceEditorViewModel>();
        }

        public string UniqueName => UNIQUE_NAME;
    }
}
