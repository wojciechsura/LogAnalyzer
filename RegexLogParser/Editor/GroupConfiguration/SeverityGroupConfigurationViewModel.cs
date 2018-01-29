using LogAnalyzer.API.Types;
using RegexLogParser.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegexLogParser.Editor.GroupConfiguration
{
    class SeverityGroupConfigurationViewModel : BaseGroupConfigurationViewModel
    {
        public SeverityGroupConfigurationViewModel(SeverityGroupDefinition groupDefinition)
        {

        }

        public SeverityGroupConfigurationViewModel()
        {

        }

        public override BaseGroupDefinition GetGroupDefinition()
        {
            return new SeverityGroupDefinition();
        }

        public override ValidationResult Validate()
        {
            return new ValidationResult(true, null);
        }
    }
}
