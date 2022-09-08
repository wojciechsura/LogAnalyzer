using LogAnalyzer.API.Models;
using LogAnalyzer.API.Types;
using LogAnalyzer.BusinessLogic.ViewModels.Processing;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using LogAnalyzer.Models.Engine;
using LogAnalyzer.Models.Views.HighlightConfigWindow;
using LogAnalyzer.Services.Common;
using LogAnalyzer.Services.Interfaces;
using Spooksoft.VisualStateManager.Conditions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Spooksoft.VisualStateManager.Commands;

namespace LogAnalyzer.BusinessLogic.ViewModels
{
    public class HighlightConfigWindowViewModel : INotifyPropertyChanged
    {
        private readonly IMessagingService messagingService;

        private ModalDialogResult<HighlightConfig> result;
        private HighlightingRuleEditorViewModel selectedRule;
        private int selectedRuleIndex = -1;
        private List<string> availableCustomColumns;

        private readonly SimpleCondition ruleSelectedCondition;
        private readonly SimpleCondition firstRuleSelectedCondition;
        private readonly SimpleCondition lastRuleSelectedCondition;
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
            var newRule = new HighlightingRuleEditorViewModel(availableCustomColumns);
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
                    messagingService.Warn(result.Message);
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

        private void LoadCurrentConfig(HighlightConfig currentConfig, HighlightEntry newEntry)
        {
            for (int i = 0; i < currentConfig.HighlightEntries.Count; i++)
            {
                HighlightingRuleEditorViewModel rule = new HighlightingRuleEditorViewModel(availableCustomColumns, currentConfig.HighlightEntries[i]);
                Rules.Add(rule);
            }

            if (newEntry != null)
            {
                HighlightingRuleEditorViewModel rule = new HighlightingRuleEditorViewModel(availableCustomColumns, newEntry);
                Rules.Add(rule);
                SelectedRule = Rules.LastOrDefault();
            }
            else
            {
                SelectedRule = Rules.FirstOrDefault();
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

            ruleSelectedCondition = new SimpleCondition(false);
            firstRuleSelectedCondition = new SimpleCondition(false);
            lastRuleSelectedCondition = new SimpleCondition(false);

            AddRuleCommand = new AppCommand((obj) => DoAddRule());
            RemoveRuleCommand = new AppCommand((obj) => DoRemoveRule(), ruleSelectedCondition);
            MoveRuleUpCommand = new AppCommand((obj) => DoMoveRuleUp(), !firstRuleSelectedCondition & ruleSelectedCondition);
            MoveRuleDownCommand = new AppCommand((obj) => DoMoveRuleDown(), !lastRuleSelectedCondition & ruleSelectedCondition);
            OkCommand = new AppCommand((obj) => DoOk());
            CancelCommand = new AppCommand((obj) => DoCancel());

            Rules = new ObservableCollection<HighlightingRuleEditorViewModel>();

            if (model.CurrentConfig != null)
                LoadCurrentConfig(model.CurrentConfig, model.NewEntry);
        }

        public ObservableCollection<HighlightingRuleEditorViewModel> Rules { get; }

        public HighlightingRuleEditorViewModel SelectedRule
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
