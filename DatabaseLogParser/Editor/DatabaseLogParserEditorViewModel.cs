using DatabaseLogParser.Configuration;
using DatabaseLogParser.Editor.FieldConfiguration;
using LogAnalyzer.API.LogParser;
using LogAnalyzer.API.Types;
using LogAnalyzer.Services.Interfaces;
using Spooksoft.VisualStateManager.Commands;
using Spooksoft.VisualStateManager.Conditions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DatabaseLogParser.Editor
{
    internal class DatabaseLogParserEditorViewModel : ILogParserEditorViewModel, INotifyPropertyChanged
    {
        // Private constants --------------------------------------------------

        private readonly string DISPLAY_NAME = "Database log parser";
        private readonly string EDITOR_RESOURCE = "DatabaseLogEditorTemplate";

        // Private fields -----------------------------------------------------

        private FieldDefinitionViewModel selectedFieldDefinition;
        private int selectedFieldDefinitionIndex;
        private readonly ObservableCollection<FieldDefinitionViewModel> fieldDefinitions;

        private readonly SimpleCondition itemSelectedCondition;
        private readonly SimpleCondition firstItemSelectedCondition;
        private readonly SimpleCondition lastItemSelectedCondition;

        // Private methods ----------------------------------------------------

        private void DoAddFieldDefinition()
        {
            var newFieldDefinition = new FieldDefinitionViewModel();
            fieldDefinitions.Add(newFieldDefinition);
            SelectedFieldDefinition = newFieldDefinition;
        }

        private void DoRemoveFieldDefinition()
        {
            fieldDefinitions.Remove(selectedFieldDefinition);
            SelectedFieldDefinition = null;
        }

        private void DoMoveLeft()
        {
            int index = fieldDefinitions.IndexOf(selectedFieldDefinition);
            fieldDefinitions.Move(index, index - 1);
        }

        private void DoMoveRight()
        {
            int index = fieldDefinitions.IndexOf(selectedFieldDefinition);
            fieldDefinitions.Move(index, index + 1);
        }

        private void Clear()
        {
            fieldDefinitions.Clear();
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Public methods -----------------------------------------------------

        public DatabaseLogParserEditorViewModel(ILogParserProvider parentProvider)
        {
            this.Provider = parentProvider;

            fieldDefinitions = new ObservableCollection<FieldDefinitionViewModel>();

            itemSelectedCondition = new SimpleCondition(false);
            firstItemSelectedCondition = new SimpleCondition(false);
            lastItemSelectedCondition = new SimpleCondition(false);

            AddFieldDefinitionCommand = new AppCommand((obj) => DoAddFieldDefinition());
            RemoveFieldDefinitionCommand = new AppCommand((obj) => DoRemoveFieldDefinition(), itemSelectedCondition);
            MoveLeftCommand = new AppCommand((obj) => DoMoveLeft(), !firstItemSelectedCondition & itemSelectedCondition);
            MoveRightCommand = new AppCommand((obj) => DoMoveRight(), !lastItemSelectedCondition & itemSelectedCondition);
        }

        public ILogParserConfiguration GetConfiguration()
        {
            if (!Validate().Valid)
                throw new InvalidOperationException("Cannot get configuration, editor values are not valid!");

            var fieldDefinitions = new List<BaseFieldDefinition>();
            for (int i = 0; i < this.fieldDefinitions.Count; i++)
            {
                BaseFieldDefinition data = this.fieldDefinitions[i].GetFieldDefinition();
                fieldDefinitions.Add(data);
            }

            var configuration = new DatabaseLogParserConfiguration
            {
                FieldDefinitions = fieldDefinitions
            };

            return configuration;
        }

        public void SetConfiguration(ILogParserConfiguration configuration)
        {
            var regexConfig = (DatabaseLogParserConfiguration)configuration;

            Clear();

            for (int i = 0; i < regexConfig.FieldDefinitions.Count; i++)
            {
                FieldDefinitionViewModel fieldDefinitionViewModel = new FieldDefinitionViewModel(regexConfig.FieldDefinitions[i]);
                fieldDefinitions.Add(fieldDefinitionViewModel);
            }
        }

        public ValidationResult Validate()
        {
            // Field count > 0
            if (fieldDefinitions.Count == 0)
                return new ValidationResult(false, "You have to define at least one field definition!");

            // Unique custom field names
            var customFieldNames = fieldDefinitions.Where(g => g.FieldConfiguration is CustomFieldConfigurationViewModel)
                    .Select(g => g.FieldConfiguration as CustomFieldConfigurationViewModel)
                    .Select(c => c.Name)
                    .ToList();
            if (customFieldNames.Count != customFieldNames.Distinct().Count())
                return new ValidationResult(false, "Custom field names must be unique!");

            // Unique field types (except custom)
            var fieldTypes = fieldDefinitions
                .Where(d => d.SelectedFieldType.Column != LogEntryColumn.Custom)
                .Select(g => g.SelectedFieldType.Column)
                .ToList();
            if (fieldTypes.Count() != fieldTypes.Distinct().Count())
                return new ValidationResult(false, "Common (non-custom) field types cannot be used more than once!");

            for (int i = 0; i < fieldDefinitions.Count; i++)
            {
                ValidationResult result = fieldDefinitions[i].Validate();
                if (!result.Valid)
                    return result;
            }

            return new ValidationResult(true, null);
        }

        public void SetSampleLines(List<string> sampleLines)
        {
            // Unsupported
        }

        // Public properties --------------------------------------------------

        public ILogParserProvider Provider { get; }

        public string DisplayName => DISPLAY_NAME;

        public string EditorResource => EDITOR_RESOURCE;

        public ObservableCollection<FieldDefinitionViewModel> FieldDefinitions => fieldDefinitions;

        public FieldDefinitionViewModel SelectedFieldDefinition
        {
            get
            {
                return selectedFieldDefinition;
            }
            set
            {
                selectedFieldDefinition = value;
                OnPropertyChanged(nameof(SelectedFieldDefinition));
            }
        }

        public int SelectedFieldDefinitionIndex
        {
            get
            {
                return selectedFieldDefinitionIndex;
            }
            set
            {
                selectedFieldDefinitionIndex = value;
                itemSelectedCondition.Value = (value >= 0);
                firstItemSelectedCondition.Value = (value == 0);
                lastItemSelectedCondition.Value = (value == fieldDefinitions.Count - 1);
                OnPropertyChanged(nameof(SelectedFieldDefinitionIndex));
            }
        }

        public ICommand AddFieldDefinitionCommand { get; }
        public ICommand RemoveFieldDefinitionCommand { get; }
        public ICommand MoveLeftCommand { get; }
        public ICommand MoveRightCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
