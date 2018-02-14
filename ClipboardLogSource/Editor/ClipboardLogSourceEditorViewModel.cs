using LogAnalyzer.API.LogSource;
using LogAnalyzer.API.Types;
using LogAnalyzer.Services.Interfaces;
using LogAnalyzer.Wpf.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ClipboardLogSource.Editor
{
    public class ClipboardLogSourceEditorViewModel : ILogSourceEditorViewModel, INotifyPropertyChanged
    {
        // Private constants --------------------------------------------------

        private const string DISPLAY_NAME = "Clipboard";
        private const string EDITOR_RESOURCE = "ClipboardLogEditorDataTemplate";

        // Private fields -----------------------------------------------------

        // ILogSourceEditorVoewModel implementation ---------------------------

        public ILogSourceConfiguration BuildConfiguration()
        {
            return new ClipboardLogSourceConfiguration();
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ValidationResult Validate()
        {
            if (Clipboard.ContainsText(TextDataFormat.Text) || Clipboard.ContainsText(TextDataFormat.CommaSeparatedValue))
                return new ValidationResult(true, null);

            return new ValidationResult(false, "Clipboard does not contain text!");
        }

        // Public methods -----------------------------------------------------

        public ClipboardLogSourceEditorViewModel(ILogSourceProvider provider)
        {
            this.Provider = provider;
        }

        public void LoadConfiguration(ILogSourceConfiguration configuration)
        {
            
        }

        // Public properties --------------------------------------------------

        public string DisplayName => DISPLAY_NAME;

        public string EditorResource => EDITOR_RESOURCE;

        public ILogSourceProvider Provider { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
