using LogAnalyzer.Models.Engine.ProcessConditions;
using LogAnalyzer.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.BusinessLogic.ViewModels.Highlighting
{
    public class DateRuleDataEditorViewModel : BaseRuleDataEditorViewModel
    {
        private DateTime argument;

        private void RemoveNonSupportedComparisonMethods()
        {
            ComparisonMethods.Remove(ComparisonMethods.Single(ci => ci.ComparisonMethod == ComparisonMethod.Contains));
            ComparisonMethods.Remove(ComparisonMethods.Single(ci => ci.ComparisonMethod == ComparisonMethod.Matches));
        }

        public DateRuleDataEditorViewModel()
        {
            RemoveNonSupportedComparisonMethods();
        }

        public DateRuleDataEditorViewModel(DateProcessCondition condition)
            : base(condition)
        {
            RemoveNonSupportedComparisonMethods();
            Argument = condition.Argument;            
        }

        public DateTime Argument
        {
            get => argument; set
            {
                argument = value;
                OnPropertyChanged(nameof(Argument));
            }
        }
    }
}
