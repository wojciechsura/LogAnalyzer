using LogAnalyzer.API.Models;
using LogAnalyzer.API.Types;
using LogAnalyzer.BusinessLogic.ViewModels.Highlighting;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using LogAnalyzer.Models.Engine;
using LogAnalyzer.Models.Views.HighlightConfigWindow;
using LogAnalyzer.Services.Common;
using LogAnalyzer.Services.Interfaces;
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
        private readonly IMessagingService messagingService;

        private ModalDialogResult<HighlightConfig> result;
        private RuleEditorViewModel selectedRule;
        private int selectedRuleIndex;
        private List<string> availableCustomColumns;

        private readonly Condition ruleSelectedCondition;
        private readonly Condition firstRuleSelectedCondition;
        private readonly Condition lastRuleSelectedCondition;
        private readonly IHighlightConfigWindowAccess access;

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
            var newRule = new RuleEditorViewModel(availableCustomColumns);
            Rules.Add(newRule);
            SelectedRule = newRule;
        }

        private void DoOk()
        {
            for (int i = 0; i < Rules.Count; i++)
            {
                ValidationResult result = Rules[i].Validate();
                if (!result.Valid)
                {
                    SelectedRule = Rules[i];
                    messagingService.Inform(result.Message);
                    return;
                }
            }

            List<HighlightEntry> entries = new List<HighlightEntry>();
            for (int i = 0; i < Rules.Count; i++)
            {
                entries.Add(Rules[i].CreateHighlightEntry());
            }

            HighlightConfig config = new HighlightConfig
            {
                HighlightEntries = entries
            };

            result.DialogResult = true;
            result.Result = config;
            access.Close(true);
        }

        private void DoCancel()
        {
            result.DialogResult = false;
            result.Result = null;
            access.Close(false);
        }

        private void LoadCurrentConfig(HighlightConfig currentConfig)
        {
            for (int i = 0; i < currentConfig.HighlightEntries.Count; i++)
            {
                RuleEditorViewModel rule = new RuleEditorViewModel(availableCustomColumns, currentConfig.HighlightEntries[i]);
                Rules.Add(rule);
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public HighlightConfigWindowViewModel(IHighlightConfigWindowAccess access, IMessagingService messagingService, HighlightConfigModel model)
        {
            this.access = access;
            this.messagingService = messagingService;

            result = new ModalDialogResult<HighlightConfig>();

            availableCustomColumns = model.CurrentColumns
                .OfType<CustomColumnInfo>()
                .Select(c => c.Name)
                .ToList();

            ruleSelectedCondition = new Condition(false);
            firstRuleSelectedCondition = new Condition(false);
            lastRuleSelectedCondition = new Condition(false);

            AddRuleCommand = new SimpleCommand((obj) => DoAddRule());
            RemoveRuleCommand = new SimpleCommand((obj) => DoRemoveRule(), ruleSelectedCondition);
            MoveRuleUpCommand = new SimpleCommand((obj) => DoMoveRuleUp(), !firstRuleSelectedCondition & ruleSelectedCondition);
            MoveRuleDownCommand = new SimpleCommand((obj) => DoMoveRuleDown(), !lastRuleSelectedCondition & ruleSelectedCondition);
            OkCommand = new SimpleCommand((obj) => DoOk());
            CancelCommand = new SimpleCommand((obj) => DoCancel());

            Rules = new ObservableCollection<RuleEditorViewModel>();

            if (model.CurrentConfig != null)
                LoadCurrentConfig(model.CurrentConfig);
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
        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
