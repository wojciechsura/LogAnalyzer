using Autofac;
using FileLogSource.Editor;
using LogAnalyzer.API.LogParser;
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
            return LogAnalyzer.Dependencies.Container.Instance.Resolve<FileLogSourceEditorViewModel>(new NamedParameter("provider", this));
        }

        public ILogSource CreateLogSource(ILogSourceConfiguration configuration, ILogParser parser)
        {
            return new FileLogSource(configuration);
        }

        public ILogSourceConfiguration CreateFromLocalPaths(List<string> files)
        {
            if (files.Count == 1)
            {
                return new FileLogSourceConfiguration(files[0]);
            }
            else
                return null;
        }

        public ILogSourceConfiguration CreateFromClipboard()
        {
            return null;
        }

        public string UniqueName => Common.Consts.UNIQUE_NAME;
    }
}
