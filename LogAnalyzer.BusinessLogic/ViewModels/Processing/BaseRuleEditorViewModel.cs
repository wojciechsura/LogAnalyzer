using LogAnalyzer.API.Types;
using LogAnalyzer.API.Types.Attributes;
using LogAnalyzer.Common.Extensions;
using LogAnalyzer.Models.Engine.PredicateDescriptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.BusinessLogic.ViewModels.Processing
{
    public abstract class BaseRuleEditorViewModel : INotifyPropertyChanged
    {
        // Public types -------------------------------------------------------

        public class LogEntryColumnInfo
        {
            public LogEntryColumnInfo(LogEntryColumn column)
            {
                Column = column;
            }

            public LogEntryColumn Column { get; }
            public string Display => Column.GetAttribute<ColumnHeaderAttribute>().Header;
        }

        // Protected fields ---------------------------------------------------

        protected LogEntryColumnInfo selectedColumn;
        protected List<string> availableCustomColumns;
        protected BaseRuleDataEditorViewModel dataEditorViewModel;

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

        protected PredicateDescription BuildProcessCondition()
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

        protected void RestoreDataEditorViewModel(PredicateDescription predicateDescription)
        {
            if (predicateDescription is DatePredicateDescription dateCondition)
            {
                selectedColumn = AvailableColumns.Single(c => c.Column == LogEntryColumn.Date);
                dataEditorViewModel = new DateRuleDataEditorViewModel(dateCondition);
            }
            else if (predicateDescription is MessagePredicateDescription messageCondition)
            {
                selectedColumn = AvailableColumns.Single(c => c.Column == LogEntryColumn.Message);
                dataEditorViewModel = new MessageRuleDataEditorViewModel(messageCondition);
            }
            else if (predicateDescription is SeverityPredicateDescription severityCondition)
            {
                selectedColumn = AvailableColumns.Single(c => c.Column == LogEntryColumn.Severity);
                dataEditorViewModel = new SeverityRuleDataEditorViewModel(severityCondition);
            }
            else if (predicateDescription is CustomPredicateDescription customCondition)
            {
                selectedColumn = AvailableColumns.Single(c => c.Column == LogEntryColumn.Custom);
                dataEditorViewModel = new CustomRuleDataEditorViewModel(availableCustomColumns, customCondition);
            }
            else
                throw new ArgumentException("Invalid highlight entry!");

            OnPropertyChanged(nameof(SelectedColumn));
            OnPropertyChanged(nameof(DataEditorViewModel));
        }

        // Public methods -----------------------------------------------------

        public BaseRuleEditorViewModel(List<string> availableCustomColumns)
        {
            this.availableCustomColumns = availableCustomColumns;

            AvailableColumns = new ObservableCollection<LogEntryColumnInfo>();
            BuildAvailableColumns();
        }

        public abstract ValidationResult Validate();

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

        public abstract string Summary { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
