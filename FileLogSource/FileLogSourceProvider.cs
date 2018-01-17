using FileLogSource.Editor;
using LogAnalyzer.API.LogSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileLogSource
{
    public class FileLogSourceProvider : ILogSourceProvider
    {
        public ILogSourceEditorViewModel CreateEditorViewModel()
        {
            return new FileSourceEditorViewModel();
        }
    }
}
