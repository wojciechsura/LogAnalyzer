using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LogAnalyzer.API.Types;
using LogAnalyzer.Models.Engine.PredicateDescriptions;
using LogAnalyzer.Models.Types;

namespace LogAnalyzer.BusinessLogic.ViewModels.Processing
{
    public class SeverityRuleDataEditorViewModel : BaseRuleDataEditorViewModel
    {
        private string argument;
        private bool caseSensitive;

        public SeverityRuleDataEditorViewModel()
        {

        }

        public SeverityRuleDataEditorViewModel(SeverityPredicateDescription condition) 
            : base(condition)
        {
            Argument = condition.Argument;
            CaseSensitive = condition.CaseSensitive;
        }

        public override ValidationResult Validate()
        {
            if (new ComparisonMethod[] { ComparisonMethod.Matches, ComparisonMethod.NotMatches }.Contains(SelectedComparisonMethod.ComparisonMethod))
            {
                try
                {
                    new Regex(argument);
                }
                catch
                {
                    return new ValidationResult(false, "Invalid regular expression for highlighting rule!");
                }
            }

            if (String.IsNullOrEmpty(argument))
            {
                return new ValidationResult(false, "Enter argument for validation rule!");
            }

            return new ValidationResult(true, null);
        }

        public string Argument
        {
            get => argument;
            set
            {
                argument = value;
                OnPropertyChanged(nameof(Argument));
                OnPropertyChanged(nameof(Summary));
            }
        }

        public bool CaseSensitive
        {
            get => caseSensitive;
            set
            {
                caseSensitive = value;
                OnPropertyChanged(nameof(CaseSensitive));
                OnPropertyChanged(nameof(Summary));
            }
        }

        public override string Summary => $"{SelectedComparisonMethod.SummaryDisplay} \"{Argument}\"{(CaseSensitive ? " (case sensitive)" : "")}";
    }
}
