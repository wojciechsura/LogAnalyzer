using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.API.Types;
using RegexLogParser.Configuration;

namespace RegexLogParser.Editor.GroupConfiguration
{
    class DateGroupConfigurationViewModel : BaseGroupConfigurationViewModel
    {
        public DateGroupConfigurationViewModel(DateGroupDefinition groupDefinition)
        {
            this.Format = groupDefinition.Format;
        }

        public DateGroupConfigurationViewModel()
        {

        }

        public override BaseGroupDefinition GetGroupDefinition()
        {
            return new DateGroupDefinition
            {
                Format = this.Format
            };
        }

        public override ValidationResult Validate()
        {
            return new ValidationResult(true, null);
        }

        public string Format { get; set; }
    }
}
