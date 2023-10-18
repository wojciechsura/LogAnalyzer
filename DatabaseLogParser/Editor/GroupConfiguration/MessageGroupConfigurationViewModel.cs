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
    class MessageFieldConfigurationViewModel : BaseFieldConfigurationViewModel
    {
        public MessageFieldConfigurationViewModel(MessageFieldDefinition fieldDefinition)
            : base(fieldDefinition)
        {

        }

        public MessageFieldConfigurationViewModel()
            : base()
        {

        }

        public override BaseFieldDefinition GetFieldDefinition()
        {
            return new MessageFieldDefinition()
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

        public override string Display => $"{Field} (Message)";
    }
}
