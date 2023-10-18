using LogAnalyzer.API.LogSource;
using LogAnalyzer.API.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLogSource.Editor
{
    public class DatabaseLogSourceEditorViewModel : ILogSourceEditorViewModel, INotifyPropertyChanged
    {
        // Private constants --------------------------------------------------

        private const string DISPLAY_NAME = "Database";
        private const string EDITOR_RESOURCE = "DatabaseLogEditorDataTemplate";

        // Private fields -----------------------------------------------------

        private string connectionString;
        private string query;

        // ILogSourceEditorViewModel implementation ---------------------------

        public ILogSourceConfiguration BuildConfiguration()
        {
            return new DatabaseLogSourceConfiguration(connectionString, query);
        }

        // Private methods ----------------------------------------------------

        private void OnSourceChanged()
        {
            SourceChanged?.Invoke(this, EventArgs.Empty);
        }

        private void LoadConfig(DatabaseLogSourceConfiguration config)
        {
            this.Query = config.Query;
            this.ConnectionString = config.ConnectionString;
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ValidationResult Validate()
        {
            if (String.IsNullOrEmpty(connectionString))
                return new ValidationResult(false, "Enter connection string!");
            if (string.IsNullOrEmpty(query)) 
                return new ValidationResult(false, "Enter query!");

            return new ValidationResult(true, null);
        }

        // Public methods -----------------------------------------------------

        public DatabaseLogSourceEditorViewModel(ILogSourceProvider provider)
        {
            this.Provider = provider;
        }

        public void LoadConfiguration(ILogSourceConfiguration configuration)
        {
            var config = (DatabaseLogSourceConfiguration)configuration;
            LoadConfig(config);
        }

        public List<string> ProvideSampleLines()
        {
            return new List<string>();
        }

        // Public properties --------------------------------------------------

        public string DisplayName => DISPLAY_NAME;

        public string EditorResource => EDITOR_RESOURCE;

        public string Query
        {
            get => query;
            set
            {
                query = value;
                OnPropertyChanged(nameof(Query));
            }
        }

        public string ConnectionString
        {
            get => connectionString;
            set
            {
                connectionString = value;
                OnPropertyChanged(nameof(ConnectionString));
            }
        }

        public ILogSourceProvider Provider { get; }

        public bool ProvidesSampleLines => false;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler SourceChanged;
    }
}
