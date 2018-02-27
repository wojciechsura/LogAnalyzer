using ClipboardLogSource.Editor;
using LogAnalyzer.API.LogParser;
using LogAnalyzer.API.LogSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Resolution;

namespace ClipboardLogSource
{
    public class ClipboardLogSourceProvider : ILogSourceProvider
    {
        public ILogSourceEditorViewModel CreateEditorViewModel()
        {
            return LogAnalyzer.Dependencies.Container.Instance.Resolve<ClipboardLogSourceEditorViewModel>(new ParameterOverride("provider", this));
        }

        public ILogSource CreateLogSource(ILogSourceConfiguration configuration, ILogParser parser)
        {
            return new ClipboardLogSource(configuration);
        }

        public ILogSourceConfiguration CreateFromLocalPaths(List<string> files)
        {
            return null;
        }

        public ILogSourceConfiguration CreateFromClipboard()
        {
            return new ClipboardLogSourceConfiguration();
        }

        public string UniqueName => Common.Consts.UNIQUE_NAME;
    }
}
