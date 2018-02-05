using LogAnalyzer.API.LogParser;
using LogAnalyzer.API.LogSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FilesLogSource.Common;
using Unity;
using Unity.Resolution;
using FilesLogSource.Editor;

namespace FilesLogSource
{
    public class FilesLogSourceProvider : ILogSourceProvider
    {
        public string UniqueName => Consts.UNIQUE_NAME;

        public ILogSourceEditorViewModel CreateEditorViewModel()
        {
            return LogAnalyzer.Dependencies.Container.Instance.Resolve<FilesLogSourceEditorViewModel>(new ParameterOverride("provider", this));
        }

        public ILogSourceConfiguration CreateFromLocalPaths(List<string> files)
        {
            return new FilesLogSourceConfiguration
            {
                Files = files,
                AutoSort = true
            };
        }

        public ILogSource CreateLogSource(ILogSourceConfiguration configuration, ILogParser logParser)
        {
            var filesConfiguration = configuration as FilesLogSourceConfiguration;
            if (filesConfiguration == null)
                throw new ArgumentException("Invalid configuration");
            if (logParser == null)
                throw new ArgumentNullException(nameof(logParser));

            return new FilesLogSource(filesConfiguration, logParser);
        }
    }
}
