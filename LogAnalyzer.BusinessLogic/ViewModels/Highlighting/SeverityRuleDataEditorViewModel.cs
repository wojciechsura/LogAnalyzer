using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.Models.Engine.PredicateDescriptions;

namespace LogAnalyzer.BusinessLogic.ViewModels.Highlighting
{
    public class SeverityRuleDataEditorViewModel : BaseRuleDataEditorViewModel
    {
        private string argument;

        public SeverityRuleDataEditorViewModel()
        {

        }

        public SeverityRuleDataEditorViewModel(SeverityPredicateDescription condition) 
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
