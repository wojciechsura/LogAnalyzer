using LogAnalyzer.API.Types;
using RegexLogParser.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegexLogParser.Editor.GroupConfiguration
{
    class MessageGroupConfigurationViewModel : BaseGroupConfigurationViewModel
    {
        public MessageGroupConfigurationViewModel(MessageGroupDefinition groupDefinition)
        {

        }

        public MessageGroupConfigurationViewModel()
        {

        }

        public override BaseGroupDefinition GetGroupDefinition()
        {
            return new MessageGroupDefinition();
        }

        public override ValidationResult Validate()
        {
            return new ValidationResult(true, null);
        }
    }
}
