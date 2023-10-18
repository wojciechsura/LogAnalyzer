using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseLogParser.Configuration;
using LogAnalyzer.API.Types;

namespace DatabaseLogParser.Editor.FieldConfiguration
{
    class CustomFieldConfigurationViewModel : BaseFieldConfigurationViewModel
    {
        private string name;

        public CustomFieldConfigurationViewModel(CustomFieldDefinition fieldDefinition)
            : base(fieldDefinition)
        {
            Name = fieldDefinition.Name;
        }

        public CustomFieldConfigurationViewModel()
            : base()
        {

        }

        public override BaseFieldDefinition GetFieldDefinition()
        {
            return new CustomFieldDefinition
            {
                Name = Name,
                Field = Field
            };
        }

        public override ValidationResult Validate()
        {
            var result = base.Validate();
            if (!result.Valid)
                return result;

            if (string.IsNullOrEmpty(Name))
                return new ValidationResult(false, "Custom field name can not be empty!");

            return new ValidationResult(true, null);
        }

        public override string Display => $"{Field} (Custom: {Name})";

        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Display));
            }
        }
    }
}
