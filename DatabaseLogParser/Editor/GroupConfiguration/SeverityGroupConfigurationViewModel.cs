using DatabaseLogParser.Configuration;
using LogAnalyzer.API.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DatabaseLogParser.Editor.FieldConfiguration
{
    class SeverityFieldConfigurationViewModel : BaseFieldConfigurationViewModel
    {
        public SeverityFieldConfigurationViewModel(SeverityFieldDefinition fieldDefinition)
            : base(fieldDefinition)
        {

        }

        public SeverityFieldConfigurationViewModel()
            : base()
        {

        }

        public override BaseFieldDefinition GetFieldDefinition()
        {
            return new SeverityFieldDefinition()
            {
                Field = Field
            };
        }

        public override ValidationResult Validate()
        {
            var result = base.Validate();
            if (!result.Valid)
                return result;

            return new ValidationResult(true, null);
        }

        public override string Display => $"{Field} (Severity)";
    }
}
