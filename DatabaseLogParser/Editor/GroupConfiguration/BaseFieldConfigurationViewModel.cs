using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseLogParser.Configuration;
using LogAnalyzer.API.Types;

namespace DatabaseLogParser.Editor.FieldConfiguration
{
    abstract class BaseFieldConfigurationViewModel : INotifyPropertyChanged
    {
        private string field;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected BaseFieldConfigurationViewModel(BaseFieldDefinition fieldDefinition)
        {
            Field = fieldDefinition.Field;
        }

        protected BaseFieldConfigurationViewModel()
        {
            Field = "Field";
        }

        internal static BaseFieldConfigurationViewModel FromFieldDefinition(BaseFieldDefinition fieldDefinition)
        {
            if (fieldDefinition is DateFieldDefinition datefieldDefinition)
            {
                return new DateFieldConfigurationViewModel(datefieldDefinition);
            }
            else if (fieldDefinition is CustomFieldDefinition customFieldDefinition)
            {
                return new CustomFieldConfigurationViewModel(customFieldDefinition);
            }
            else if (fieldDefinition is MessageFieldDefinition messageFieldDefinition)
            {
                return new MessageFieldConfigurationViewModel(messageFieldDefinition);
            }
            else if (fieldDefinition is SeverityFieldDefinition severityFieldDefinition)
            {
                return new SeverityFieldConfigurationViewModel(severityFieldDefinition);
            }
            else
                throw new ArgumentException("Invalid field definition!");
        }

        public abstract BaseFieldDefinition GetFieldDefinition();

        public virtual ValidationResult Validate()
        {
            if (string.IsNullOrEmpty(field))
                return new ValidationResult(false, "Please enter field from query result!");

            return new ValidationResult(true, null);
        }

        public string Field
        {
            get => field;
            set
            {
                field = value;
                OnPropertyChanged(nameof(Field));
                OnPropertyChanged(nameof(Display));
            }
        }

        public abstract string Display { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
