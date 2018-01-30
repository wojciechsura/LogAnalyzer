using LogAnalyzer.API.Models;
using LogAnalyzer.BusinessLogic.ViewModels.Highlighting;
using LogAnalyzer.Models.Engine;
using LogAnalyzer.Models.Views.HighlightConfigWindow;
using LogAnalyzer.Services.Common;
using LogAnalyzer.Wpf.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LogAnalyzer.BusinessLogic.ViewModels
{
    public class HighlightConfigWindowViewModel : INotifyPropertyChanged
    {
        private ModalDialogResult<HighlightConfig> result;
        private RuleEditorViewModel selectedRule;
        private int selectedRuleIndex;
        private List<BaseColumnInfo> availableColumns;

        private readonly Condition ruleSelectedCondition;
        private readonly Condition firstRuleSelectedCondition;
        private readonly Condition lastRuleSelectedCondition;

        private void DoMoveRuleDown()
        {
            Rules.Move(selectedRuleIndex, selectedRuleIndex + 1);
        }

        private void DoMoveRuleUp()
        {
            Rules.Move(selectedRuleIndex, selectedRuleIndex - 1);
        }

        private void DoRemoveRule()
        {
            Rules.Remove(selectedRule);
            SelectedRule = null;
        }

        private void DoAddRule()
        {
            var newRule = new RuleEditorViewModel(availableColumns);
            Rules.Add(newRule);
            SelectedRule = newRule;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public HighlightConfigWindowViewModel(HighlightConfigModel model)
        {
            result = new ModalDialogResult<HighlightConfig>();

            availableColumns = model.CurrentColumns;

            ruleSelectedCondition = new Condition(false);
            firstRuleSelectedCondition = new Condition(false);
            lastRuleSelectedCondition = new Condition(false);

            AddRuleCommand = new SimpleCommand((obj) => DoAddRule());
            RemoveRuleCommand = new SimpleCommand((obj) => DoRemoveRule(), ruleSelectedCondition);
            MoveRuleUpCommand = new SimpleCommand((obj) => DoMoveRuleUp(), !firstRuleSelectedCondition & ruleSelectedCondition);
            MoveRuleDownCommand = new SimpleCommand((obj) => DoMoveRuleDown(), !lastRuleSelectedCondition & ruleSelectedCondition);

            Rules = new ObservableCollection<RuleEditorViewModel>();
        }

        public ObservableCollection<RuleEditorViewModel> Rules { get; }

        public RuleEditorViewModel SelectedRule
        {
            get
            {
                return selectedRule;
            }
            set
            {
                selectedRule = value;
                OnPropertyChanged(nameof(SelectedRule));
            }
        }

        public int SelectedRuleIndex
        {
            get
            {
                return selectedRuleIndex;
            }
            set
            {
                selectedRuleIndex = value;
                ruleSelectedCondition.Value = (value >= 0);
                firstRuleSelectedCondition.Value = (value == 0);
                lastRuleSelectedCondition.Value = (value == Rules.Count - 1);
                OnPropertyChanged(nameof(SelectedRuleIndex));
            }
        }

        public ModalDialogResult<HighlightConfig> Result => result;

        public ICommand AddRuleCommand { get; }
        public ICommand RemoveRuleCommand { get; }
        public ICommand MoveRuleUpCommand { get; }
        public ICommand MoveRuleDownCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
