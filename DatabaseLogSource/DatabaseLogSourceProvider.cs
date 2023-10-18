using Autofac;
using DatabaseLogSource.Common;
using DatabaseLogSource.Editor;
using LogAnalyzer.API.LogParser;
using LogAnalyzer.API.LogSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLogSource
{
    public class DatabaseLogSourceProvider : ILogSourceProvider
    {
        public ILogSourceEditorViewModel CreateEditorViewModel()
        {
            return LogAnalyzer.Dependencies.Container.Instance.Resolve<DatabaseLogSourceEditorViewModel>(new NamedParameter("provider", this));
        }

        public ILogSourceConfiguration CreateFromClipboard()
        {
            return null;
        }

        public ILogSourceConfiguration CreateFromLocalPaths(List<string> files)
        {
            return null;
        }

        public ILogSource CreateLogSource(ILogSourceConfiguration configuration, ILogParser logParser)
        {
            return new DatabaseLogSource(configuration);
        }

        public string UniqueName => Consts.UNIQUE_NAME;
    }
}
