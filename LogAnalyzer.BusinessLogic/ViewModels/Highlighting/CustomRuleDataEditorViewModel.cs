using LogAnalyzer.Models.Engine.PredicateDescriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.BusinessLogic.ViewModels.Highlighting
{
    public class CustomRuleDataEditorViewModel : BaseRuleDataEditorViewModel
    {
        private string customField;
        private string argument;
        private bool caseSensitive;

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
