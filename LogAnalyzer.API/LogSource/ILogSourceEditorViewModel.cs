using LogAnalyzer.API.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.API.LogSource
{
    public interface ILogSourceEditorViewModel
    {
        ILogSourceConfiguration BuildConfiguration();
        void LoadConfiguration(ILogSourceConfiguration configuration);
        ValidationResult Validate();
        List<string> ProvideSampleLines();

        string DisplayName { get; }
        string EditorResource { get; }
        bool ProvidesSampleLines { get; }

        ILogSourceProvider Provider { get; }
    }
}
