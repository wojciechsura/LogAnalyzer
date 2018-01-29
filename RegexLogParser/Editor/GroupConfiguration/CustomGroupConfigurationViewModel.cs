using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.API.Types;
using RegexLogParser.Configuration;

namespace RegexLogParser.Editor.GroupConfiguration
{
    class CustomGroupConfigurationViewModel : BaseGroupConfigurationViewModel
    {
        public CustomGroupConfigurationViewModel(CustomGroupDefinition groupDefinition)
        {
            this.Name = groupDefinition.Name;
        }

        public CustomGroupConfigurationViewModel()
        {

        }

        public override BaseGroupDefinition GetGroupDefinition()
        {
            return new CustomGroupDefinition
            {
                Name = this.Name
            };
        }

        public override ValidationResult Validate()
        {
            if (String.IsNullOrEmpty(Name))
                return new ValidationResult(false, "Custom group name can not be empty!");

            return new ValidationResult(true, null);
        }

        public string Name { get; set; }        
    }
}
