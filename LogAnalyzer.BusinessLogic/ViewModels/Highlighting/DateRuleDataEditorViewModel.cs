using LogAnalyzer.API.Types;
using LogAnalyzer.Models.Engine.PredicateDescriptions;
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
            ComparisonMethods.Remove(ComparisonMethods.Single(ci => ci.ComparisonMethod == ComparisonMethod.NotContains));
            ComparisonMethods.Remove(ComparisonMethods.Single(ci => ci.ComparisonMethod == ComparisonMethod.NotMatches));
        }

        public DateRuleDataEditorViewModel()
        {
            RemoveNonSupportedComparisonMethods();
        }

        public DateRuleDataEditorViewModel(DatePredicateDescription condition)
            : base(condition)
        {
            RemoveNonSupportedComparisonMethods();
            Argument = condition.Argument;            
        }

        public override ValidationResult Validate()
        {
            if (new ComparisonMethod[] {
                    ComparisonMethod.Contains,
                    ComparisonMethod.NotContains,
                    ComparisonMethod.Matches,
                    ComparisonMethod.NotMatches
                }.Contains(SelectedComparisonMethod.ComparisonMethod))
            {
                return new ValidationResult(false, "Invalid comparison method for date!");
            }

            return new ValidationResult(true, null);
        }

        public DateTime Argument
        {
            get => argument; set
            {
                argument = value;
                OnPropertyChanged(nameof(Argument));
                OnPropertyChanged(nameof(Summary));
            }
        }

        public override string Summary => $"{SelectedComparisonMethod.SummaryDisplay} \"{Argument}\"";
    }
}
