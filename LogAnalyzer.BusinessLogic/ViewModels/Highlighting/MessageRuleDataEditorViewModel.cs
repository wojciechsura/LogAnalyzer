using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.Models.Engine.PredicateDescriptions;

namespace LogAnalyzer.BusinessLogic.ViewModels.Highlighting
{
    public class MessageRuleDataEditorViewModel : BaseRuleDataEditorViewModel
    {
        private string argument;
        private bool caseSensitive;

        public MessageRuleDataEditorViewModel()
        {

        }

        public MessageRuleDataEditorViewModel(MessagePredicateDescription condition) 
            : base(condition)
        {
            Argument = condition.Argument;
            CaseSensitive = condition.CaseSensitive;
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
