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

namespace LogAnalyzer.BusinessLogic.ViewModels.Highlighting
{
    public class RuleEditorViewModel : INotifyPropertyChanged
    {
        // Private classes ----------------------------------------------------

        public class LogEntryColumnInfo
        {
            public LogEntryColumnInfo(LogEntryColumn column)
            {
                Column = column;
            }

            public LogEntryColumn Column { get; }
            public string Display => Column.GetAttribute<ColumnHeaderAttribute>().Header;
        }

        // Private fields -----------------------------------------------------

        private LogEntryColumnInfo selectedColumn;
        private List<string> availableCustomColumns;
        private BaseRuleDataEditorViewModel dataEditorViewModel;
        private Color foreground;
        private Color background;

        // Private methods ----------------------------------------------------

        private void HandleSelectedColumnChanged()
        {
            OnPropertyChanged(nameof(SelectedColumn));
            OnPropertyChanged(nameof(Summary));

            switch (selectedColumn.Column)
            {
                case LogEntryColumn.Message:
                    {
                        DataEditorViewModel = new MessageRuleDataEditorViewModel();
                        break;
                    }
                case LogEntryColumn.Severity:
                    {
                        DataEditorViewModel = new SeverityRuleDataEditorViewModel();
                        break;
                    }
                case LogEntryColumn.Date:
                    {
                        DataEditorViewModel = new DateRuleDataEditorViewModel();
                        break;
                    }
                case LogEntryColumn.Custom:
                    {
                        DataEditorViewModel = new CustomRuleDataEditorViewModel(availableCustomColumns);
                        break;
                    }
                default:
                    throw new InvalidEnumArgumentException("Unsupported LogEntryColumn!");
            }            
        }

        private PredicateDescription BuildProcessCondition()
        {
            switch (selectedColumn.Column)
            {
                case LogEntryColumn.Date:
                    {
                        var editor = (dataEditorViewModel as DateRuleDataEditorViewModel);
                        if (editor == null)
                            throw new InvalidOperationException("Empty rule editor!");

                        return new DatePredicateDescription
                        {
                            Argument = editor.Argument,
                            Comparison = editor.SelectedComparisonMethod.ComparisonMethod
                        };
                    }
                case LogEntryColumn.Severity:
                    {
                        var editor = (dataEditorViewModel as SeverityRuleDataEditorViewModel);
                        if (editor == null)
                            throw new InvalidOperationException("Empty rule editor!");

                        return new SeverityPredicateDescription
                        {
                            Argument = editor.Argument,
                            CaseSensitive = editor.CaseSensitive,
                            Comparison = editor.SelectedComparisonMethod.ComparisonMethod
                        };
                    }
                case LogEntryColumn.Message:
                    {
                        var editor = (dataEditorViewModel as MessageRuleDataEditorViewModel);
                        if (editor == null)
                            throw new InvalidOperationException("Empty rule editor!");

                        return new MessagePredicateDescription
                        {
                            Argument = editor.Argument,
                            CaseSensitive = editor.CaseSensitive,
                            Comparison = editor.SelectedComparisonMethod.ComparisonMethod
                        };
                    }
                case LogEntryColumn.Custom:
                    {
                        var editor = (dataEditorViewModel as CustomRuleDataEditorViewModel);
                        if (editor == null)
                            throw new InvalidOperationException("Empty rule editor!");

                        return new CustomPredicateDescription
                        {
                            Name = editor.CustomField,
                            Argument = editor.Argument,
                            CaseSensitive = editor.CaseSensitive,
                            Comparison = editor.SelectedComparisonMethod.ComparisonMethod
                        };
                    }
                default:
                    throw new InvalidEnumArgumentException("Unsupported column type!");
            }
        }

        private void RestoreProcessCondition(HighlightEntry highlightEntry)
        {
            foreground = highlightEntry.Foreground;
            background = highlightEntry.Background;

            if (highlightEntry.Condition is DatePredicateDescription dateCondition)
            {
                selectedColumn = AvailableColumns.Single(c => c.Column == LogEntryColumn.Date);
                dataEditorViewModel = new DateRuleDataEditorViewModel(dateCondition);
            }
            else if (highlightEntry.Condition is MessagePredicateDescription messageCondition)
            {
                selectedColumn = AvailableColumns.Single(c => c.Column == LogEntryColumn.Message);
                dataEditorViewModel = new MessageRuleDataEditorViewModel(messageCondition);
            }
            else if (highlightEntry.Condition is SeverityPredicateDescription severityCondition)
            {
                selectedColumn = AvailableColumns.Single(c => c.Column == LogEntryColumn.Severity);
                dataEditorViewModel = new SeverityRuleDataEditorViewModel(severityCondition);
            }
            else if (highlightEntry.Condition is CustomPredicateDescription customCondition)
            {
                selectedColumn = AvailableColumns.Single(c => c.Column == LogEntryColumn.Custom);
                dataEditorViewModel = new CustomRuleDataEditorViewModel(availableCustomColumns, customCondition);
            }
            else
                throw new ArgumentException("Invalid highlight entry!");

            OnPropertyChanged(nameof(Foreground));
            OnPropertyChanged(nameof(Background));
            OnPropertyChanged(nameof(DataEditorViewModel));
            OnPropertyChanged(nameof(SelectedColumn));
        }

        private void BuildAvailableColumns()
        {           
            foreach (LogEntryColumn column in Enum.GetValues(typeof(LogEntryColumn)))
            {
                AvailableColumns.Add(new LogEntryColumnInfo(column));
            }
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Public methods -----------------------------------------------------

        public RuleEditorViewModel(List<string> availableCustomColumns)
        {
            AvailableColumns = new ObservableCollection<LogEntryColumnInfo>();
            BuildAvailableColumns();

            this.availableCustomColumns = availableCustomColumns;

            SelectedColumn = AvailableColumns.First();
            Foreground = Colors.Black;
            Background = Colors.Transparent;
        }

        public RuleEditorViewModel(List<string> availableCustomColumns, HighlightEntry highlightEntry)
        {
            AvailableColumns = new ObservableCollection<LogEntryColumnInfo>();
            BuildAvailableColumns();

            this.availableCustomColumns = availableCustomColumns;

            RestoreProcessCondition(highlightEntry);
        }

        public HighlightEntry CreateHighlightEntry()
        {
            PredicateDescription condition = BuildProcessCondition();
            
            HighlightEntry result = new HighlightEntry
            {
                Foreground = this.Foreground,
                Background = this.Background,
                Condition = condition
            };

            return result;
        }

        // Public properties --------------------------------------------------

        public ObservableCollection<LogEntryColumnInfo> AvailableColumns { get; }

        public LogEntryColumnInfo SelectedColumn
        {
            get
            {
                return selectedColumn;
            }
            set
            {
                selectedColumn = value;
                HandleSelectedColumnChanged();            
            }
        }

        public BaseRuleDataEditorViewModel DataEditorViewModel
        {
            get => dataEditorViewModel;
            set
            {
                dataEditorViewModel = value;
                OnPropertyChanged(nameof(DataEditorViewModel));
            }
        }

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

        public string Summary
        {
            get
            {
                return selectedColumn.Display.ToLower();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
