using LogAnalyzer.API.Models;
using LogAnalyzer.API.Types;
using LogAnalyzer.API.Types.Attributes;
using LogAnalyzer.BusinessLogic.ViewModels.Processing;
using LogAnalyzer.Common.Extensions;
using LogAnalyzer.Models.Engine;
using LogAnalyzer.Models.Engine.PredicateDescriptions;
using LogAnalyzer.Models.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LogAnalyzer.BusinessLogic.ViewModels.Processing
{
    public class FilteringRuleEditorViewModel : BaseRuleEditorViewModel
    {
        // Public types -------------------------------------------------------

        public class FilterActionInfo
        {
            public FilterActionInfo(FilterAction action)
            {
                Action = action;
            }

            public FilterAction Action { get; }
            public string Display => Action.GetAttribute<DescriptionAttribute>().Description;
        }

        // Private fields -----------------------------------------------------

        private readonly ObservableCollection<FilterActionInfo> availableFilterActions;
        private FilterActionInfo selectedFilterAction;

        // Private methods ----------------------------------------------------       

        private void BuildAvailableFilterActions()
        {
            foreach (FilterAction action in Enum.GetValues(typeof(FilterAction)))
            {
                availableFilterActions.Add(new FilterActionInfo(action));
            }
        }
        private void RestoreProcessCondition(FilterEntry filterEntry)
        {
            RestoreDataEditorViewModel(filterEntry.PredicateDescription);

            selectedFilterAction = availableFilterActions.Single(a => a.Action == filterEntry.Action);
            OnPropertyChanged(nameof(SelectedFilterAction));
        }
       
        // Public methods -----------------------------------------------------

        public FilteringRuleEditorViewModel(List<string> availableCustomColumns)
            : base(availableCustomColumns)
        {
            availableFilterActions = new ObservableCollection<FilterActionInfo>();
            BuildAvailableFilterActions();

            SelectedColumn = AvailableColumns.First();
            SelectedFilterAction = availableFilterActions.Single(a => a.Action == FilterAction.Include);
        }

        public FilteringRuleEditorViewModel(List<string> availableCustomColumns, FilterEntry filterEntry)
            : base(availableCustomColumns)
        {
            availableFilterActions = new ObservableCollection<FilterActionInfo>();
            BuildAvailableFilterActions();

            RestoreProcessCondition(filterEntry);
        }

        public override ValidationResult Validate()
        {
            if (selectedColumn == null)
                return new ValidationResult(false, "Select field for filtering rule");

            return dataEditorViewModel.Validate();
        }

        public FilterEntry CreateFilterEntry()
        {
            PredicateDescription condition = BuildProcessCondition();
            
            FilterEntry result = new FilterEntry
            {
                Action = SelectedFilterAction.Action,
                PredicateDescription = condition
            };

            return result;
        }

        // Public properties --------------------------------------------------

        public ObservableCollection<FilterActionInfo> AvailableFilterActions => availableFilterActions;

        public FilterActionInfo SelectedFilterAction
        {
            get => selectedFilterAction;
            set
            {
                selectedFilterAction = value;
                OnPropertyChanged(nameof(SelectedFilterAction));
                OnPropertyChanged(nameof(ActionSummary));
            }            
        }

        public override string Summary
        {
            get
            {
                return selectedColumn.Display.ToLower();
            }
        }

        public string ActionSummary
        {
            get
            {
                return SelectedFilterAction.Action.GetAttribute<SummaryDisplayAttribute>().Summary;
            }
        }
    }
}
