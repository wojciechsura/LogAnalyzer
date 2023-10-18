using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseLogParser.Configuration;
using DatabaseLogParser.Editor.FieldConfiguration;
using LogAnalyzer.API.Types;
using LogAnalyzer.API.Types.Attributes;
using LogAnalyzer.Common.Extensions;

namespace DatabaseLogParser.Editor
{
    class FieldDefinitionViewModel : INotifyPropertyChanged
    {
        public class FieldDisplayInfo
        {
            public FieldDisplayInfo(LogEntryColumn column, string name)
            {
                Column = column;
                DisplayName = name;
            }

            public LogEntryColumn Column { get; }
            public string DisplayName { get; }
        }

        private string displayName;
        private BaseFieldConfigurationViewModel fieldConfiguration;
        private readonly List<FieldDisplayInfo> availableFieldTypes;
        private FieldDisplayInfo selectedFieldType;

        private void FillAvailableFieldTypes()
        {
            foreach (LogEntryColumn column in Enum.GetValues(typeof(LogEntryColumn)))
            {
                availableFieldTypes.Add(new FieldDisplayInfo(column,
                    column.GetAttribute<ColumnHeaderAttribute>().Header));
            }
        }

        private void HandleSelectedColumnTypeChanged(FieldDisplayInfo value)
        {
            DisplayName = value.DisplayName;
            switch (value.Column)
            {
                case LogEntryColumn.Date:
                    {
                        FieldConfiguration = new DateFieldConfigurationViewModel();
                        break;
                    }
                case LogEntryColumn.Custom:
                    {
                        FieldConfiguration = new CustomFieldConfigurationViewModel();
                        break;
                    }
                case LogEntryColumn.Message:
                    {
                        FieldConfiguration = new MessageFieldConfigurationViewModel();
                        break;
                    }
                case LogEntryColumn.Severity:
                    {
                        FieldConfiguration = new SeverityFieldConfigurationViewModel();
                        break;
                    }
                default:
                    throw new InvalidOperationException("Not recognized log entry column!");
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public FieldDefinitionViewModel()
        {
            availableFieldTypes = new List<FieldDisplayInfo>();
            FillAvailableFieldTypes();

            SelectedFieldType = availableFieldTypes[0];
        }

        public FieldDefinitionViewModel(BaseFieldDefinition fieldDefinition)
        {
            availableFieldTypes = new List<FieldDisplayInfo>();
            FillAvailableFieldTypes();

            fieldConfiguration = BaseFieldConfigurationViewModel.FromFieldDefinition(fieldDefinition);
            selectedFieldType = availableFieldTypes.Single(a => fieldDefinition.GetColumn() == a.Column);
            displayName = selectedFieldType.DisplayName;

            OnPropertyChanged(nameof(FieldConfiguration));
            OnPropertyChanged(nameof(SelectedFieldType));
            OnPropertyChanged(nameof(DisplayName));
        }

        public string DisplayName
        {
            get
            {
                return displayName;
            }
            set
            {
                displayName = value;
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        public List<FieldDisplayInfo> AvailableFields => availableFieldTypes;

        public FieldDisplayInfo SelectedFieldType
        {
            get
            {
                return selectedFieldType;
            }
            set
            {
                selectedFieldType = value;
                HandleSelectedColumnTypeChanged(value);
                OnPropertyChanged(nameof(SelectedFieldType));
            }
        }

        public BaseFieldDefinition GetFieldDefinition()
        {
            return fieldConfiguration.GetFieldDefinition();
        }

        public ValidationResult Validate()
        {
            return fieldConfiguration.Validate();
        }

        public BaseFieldConfigurationViewModel FieldConfiguration
        {
            get
            {
                return fieldConfiguration;
            }
            set
            {
                fieldConfiguration = value;
                OnPropertyChanged(nameof(fieldConfiguration));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
