using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.API.Types;
using RegexLogParser.Configuration;

namespace RegexLogParser.Editor.GroupConfiguration
{
    abstract class BaseGroupConfigurationViewModel
    {
        internal static BaseGroupConfigurationViewModel FromGroupDefinition(BaseGroupDefinition groupDefinition)
        {
            if (groupDefinition is DateGroupDefinition dateGroupDefinition)
            {
                return new DateGroupConfigurationViewModel(dateGroupDefinition);
            }
            else if (groupDefinition is CustomGroupDefinition customGroupDefinition)
            {
                return new CustomGroupConfigurationViewModel(customGroupDefinition);
            }
            else if (groupDefinition is MessageGroupDefinition messageGroupDefinition)
            {
                return new MessageGroupConfigurationViewModel(messageGroupDefinition);
            }
            else if (groupDefinition is SeverityGroupDefinition severityGroupDefinition)
            {
                return new SeverityGroupConfigurationViewModel(severityGroupDefinition);
            }
            else
                throw new ArgumentException("Invalid group definition!");
        }

        public abstract BaseGroupDefinition GetGroupDefinition();

        public abstract ValidationResult Validate();        
    }
}
