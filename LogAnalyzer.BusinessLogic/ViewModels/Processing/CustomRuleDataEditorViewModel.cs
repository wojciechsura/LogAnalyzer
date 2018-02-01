using LogAnalyzer.API.Types;
using LogAnalyzer.Models.Engine.PredicateDescriptions;
using LogAnalyzer.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogAnalyzer.BusinessLogic.ViewModels.Processing
{
    public class CustomRuleDataEditorViewModel : BaseRuleDataEditorViewModel
    {
        // Private fields -----------------------------------------------------

        private string customField;
        private string argument;
        private bool caseSensitive;

        // Public methods -----------------------------------------------------

        public CustomRuleDataEditorViewModel(List<string> availableCustomFields)
        {
            AvailableCustomFields = availableCustomFields;
            CustomField = AvailableCustomFields.FirstOrDefault();
        }

        public CustomRuleDataEditorViewModel(List<string> availableCustomFields, CustomPredicateDescription condition)
            : base(condition)
        {
            AvailableCustomFields = availableCustomFields;
            CustomField = condition.Name;
            Argument = condition.Argument;
            CaseSensitive = condition.CaseSensitive;
        }

        public override ValidationResult Validate()
        {
            if (String.IsNullOrEmpty(customField))
            {
                return new ValidationResult(false, "Choose custom field for highlighting rule!");
            }

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

        // Public properties --------------------------------------------------

        public List<string> AvailableCustomFields { get; }

        public string CustomField
        {
            get => customField;
            set
            {
                customField = value;
                OnPropertyChanged(nameof(CustomField));
                OnPropertyChanged(nameof(Summary));
            }
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

        public override string Summary => $"\"{CustomField}\" {SelectedComparisonMethod.SummaryDisplay} \"{Argument}\"{(CaseSensitive ? " (case sensitive)" : "")}";
    }
}
