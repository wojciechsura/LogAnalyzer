using LogAnalyzer.API.Models;
using LogAnalyzer.API.Types;
using LogAnalyzer.API.Types.Attributes;
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
    public class HighlightingRuleEditorViewModel : BaseRuleEditorViewModel
    {
        // Private fields -----------------------------------------------------

        private Color foreground;
        private Color background;

        // Private methods ----------------------------------------------------

        private void RestoreProcessCondition(HighlightEntry highlightEntry)
        {
            RestoreDataEditorViewModel(highlightEntry.PredicateDescription);
            
            foreground = highlightEntry.Foreground;
            background = highlightEntry.Background;
            OnPropertyChanged(nameof(Foreground));
            OnPropertyChanged(nameof(Background));
        }

        // Public methods -----------------------------------------------------

        public HighlightingRuleEditorViewModel(List<string> availableCustomColumns)
            : base(availableCustomColumns)
        {
            // Select "Message contains" by default
            SelectedColumn = AvailableColumns.FirstOrDefault(c => c.Column == LogEntryColumn.Message);
            dataEditorViewModel.SelectedComparisonMethod = dataEditorViewModel.ComparisonMethods.FirstOrDefault(m => m.ComparisonMethod == ComparisonMethod.Contains);

            Foreground = Colors.Black;
            Background = Colors.Transparent;
        }

        public HighlightingRuleEditorViewModel(List<string> availableCustomColumns, HighlightEntry highlightEntry)
            : base(availableCustomColumns)
        {
            RestoreProcessCondition(highlightEntry);
        }

        public override ValidationResult Validate()
        {
            if (selectedColumn == null)
                return new ValidationResult(false, "Select field for highlighting rule");

            return dataEditorViewModel.Validate();
        }

        public HighlightEntry CreateHighlightEntry()
        {
            PredicateDescription condition = BuildProcessCondition();
            
            HighlightEntry result = new HighlightEntry
            {
                Foreground = this.Foreground,
                Background = this.Background,
                PredicateDescription = condition
            };

            return result;
        }

        // Public properties --------------------------------------------------

        public Color Foreground
        {
            get => foreground;
            set
            {
                foreground = value;
                OnPropertyChanged(nameof(Foreground));
            }
        }

        public Color Background
        {
            get => background;
            set
            {
                background = value;
                OnPropertyChanged(nameof(Background));
            }
        }

        public override string Summary
        {
            get
            {
                return selectedColumn.Display.ToLower();
            }
        }
    }
}
