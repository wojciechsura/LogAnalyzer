using LogAnalyzer.API.Models;
using LogAnalyzer.API.Types;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using LogAnalyzer.BusinessLogic.ViewModels.Processing;
using LogAnalyzer.Common.Extensions;
using LogAnalyzer.Models.Engine;
using LogAnalyzer.Models.Types;
using LogAnalyzer.Models.Views.FilterConfigWindow;
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
    public class FilterConfigWindowViewModel
    {
        public class FilterActionInfo
        {
            public FilterActionInfo(FilterAction action)
            {
                Action = action;
            }

            public FilterAction Action { get; }
            public string Display => Action.GetAttribute<DescriptionAttribute>().Description;
        }

        private readonly IMessagingService messagingService;

        private ModalDialogResult<FilterConfig> result;
        private FilteringRuleEditorViewModel selectedRule;
        private int selectedRuleIndex;
        private ObservableCollection<FilterActionInfo> availableDefaultActions;
        private FilterActionInfo selectedDefaultAction;
        private List<string> availableCustomColumns;
        private readonly Condition ruleSelectedCondition;
        private readonly Condition firstRuleSelectedCondition;
        private readonly Condition lastRuleSelectedCondition;
        private readonly IFilterConfigWindowAccess access;

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
            var newRule = new FilteringRuleEditorViewModel(availableCustomColumns);
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

            List<FilterEntry> entries = new List<FilterEntry>();
            for (int i = 0; i < Rules.Count; i++)
            {
                entries.Add(Rules[i].CreateFilterEntry());
            }

            FilterConfig config = new FilterConfig
            {
                FilterEntries = entries,
                DefaultAction = selectedDefaultAction.Action
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

        private void LoadCurrentConfig(FilterConfig currentConfig)
        {
            SelectedDefaultAction = AvailableDefaultActions.Single(a => a.Action == currentConfig.DefaultAction);

            for (int i = 0; i < currentConfig.FilterEntries.Count; i++)
            {
                FilteringRuleEditorViewModel rule = new FilteringRuleEditorViewModel(availableCustomColumns, currentConfig.FilterEntries[i]);
                Rules.Add(rule);
            }

            SelectedRule = Rules.FirstOrDefault();
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public FilterConfigWindowViewModel(IFilterConfigWindowAccess access, IMessagingService messagingService, FilterConfigModel model)
        {
            this.access = access;
            this.messagingService = messagingService;

            result = new ModalDialogResult<FilterConfig>();

            availableCustomColumns = model.CurrentColumns
                .OfType<CustomColumnInfo>()
                .Select(c => c.Name)
                .ToList();

            availableDefaultActions = new ObservableCollection<FilterActionInfo>();
            foreach (FilterAction action in Enum.GetValues(typeof(FilterAction)))
            {
                availableDefaultActions.Add(new FilterActionInfo(action));
            }
            selectedDefaultAction = availableDefaultActions.Single(a => a.Action == FilterAction.Include);
                
            ruleSelectedCondition = new Condition(false);
            firstRuleSelectedCondition = new Condition(false);
            lastRuleSelectedCondition = new Condition(false);

            AddRuleCommand = new SimpleCommand((obj) => DoAddRule());
            RemoveRuleCommand = new SimpleCommand((obj) => DoRemoveRule(), ruleSelectedCondition);
            MoveRuleUpCommand = new SimpleCommand((obj) => DoMoveRuleUp(), !firstRuleSelectedCondition & ruleSelectedCondition);
            MoveRuleDownCommand = new SimpleCommand((obj) => DoMoveRuleDown(), !lastRuleSelectedCondition & ruleSelectedCondition);
            OkCommand = new SimpleCommand((obj) => DoOk());
            CancelCommand = new SimpleCommand((obj) => DoCancel());

            Rules = new ObservableCollection<FilteringRuleEditorViewModel>();

            if (model.CurrentConfig != null)
                LoadCurrentConfig(model.CurrentConfig);
        }

        public ObservableCollection<FilteringRuleEditorViewModel> Rules { get; }

        public FilteringRuleEditorViewModel SelectedRule
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

        public ObservableCollection<FilterActionInfo> AvailableDefaultActions => availableDefaultActions;
        public FilterActionInfo SelectedDefaultAction
        {
            get => selectedDefaultAction;
            set
            {
                selectedDefaultAction = value;
                OnPropertyChanged(nameof(SelectedDefaultAction));
            }
        }

        public ModalDialogResult<FilterConfig> Result => result;

        public ICommand AddRuleCommand { get; }
        public ICommand RemoveRuleCommand { get; }
        public ICommand MoveRuleUpCommand { get; }
        public ICommand MoveRuleDownCommand { get; }
        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
