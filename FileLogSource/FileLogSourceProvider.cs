using FileLogSource.Editor;
using LogAnalyzer.API.LogParser;
using LogAnalyzer.API.LogSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Resolution;

namespace FileLogSource
{
    public class FileLogSourceProvider : ILogSourceProvider
    {
        public ILogSourceEditorViewModel CreateEditorViewModel()
        {
            return LogAnalyzer.Dependencies.Container.Instance.Resolve<FileSourceEditorViewModel>(new ParameterOverride("provider", this));
        }

        public ILogSource CreateLogSource(ILogSourceConfiguration configuration, ILogParser parser)
        {
            return new FileLogSource(configuration);
        }

        public string UniqueName => Common.Consts.UNIQUE_NAME;
    }
}
