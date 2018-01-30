using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.Models.Engine.ProcessConditions;

namespace LogAnalyzer.BusinessLogic.ViewModels.Highlighting
{
    public class MessageRuleDataEditorViewModel : BaseRuleDataEditorViewModel
    {
        private string argument;

        public MessageRuleDataEditorViewModel()
        {

        }

        public MessageRuleDataEditorViewModel(MessageProcessCondition condition) 
            : base(condition)
        {
            Argument = condition.Argument;
        }

        public string Argument
        {
            get => argument;
            set
            {
                argument = value;
                OnPropertyChanged(nameof(Argument));
            }
        }
    }
}
