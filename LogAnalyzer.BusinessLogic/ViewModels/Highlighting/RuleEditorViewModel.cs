using LogAnalyzer.API.Models;
using LogAnalyzer.API.Types;
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
        // Private fields -----------------------------------------------------

        private BaseColumnInfo selectedColumn;
        private BaseRuleDataEditorViewModel dataEditorViewModel;
        private Color foreground;
        private Color background;

        // Private methods ----------------------------------------------------

        private void HandleSelectedColumnChanged()
        {
            OnPropertyChanged(nameof(SelectedColumn));

            if (selectedColumn is CommonColumnInfo commonInfo)
            {
                switch (commonInfo.Column)
                {
                    case LogEntryColumn.Message:
                    case LogEntryColumn.Severity:
                        {
                            DataEditorViewModel = new StringRuleDataEditorViewModel();
                            break;
                        }
                    case LogEntryColumn.Date:
                        {
                            DataEditorViewModel = new DateRuleDataEditorViewModel();
                            break;
                        }
                    case LogEntryColumn.Custom:
                        throw new InvalidOperationException("Common column cannot containt Custom LogEntryColumn!");
                    default:
                        throw new InvalidEnumArgumentException("Unsupported LogEntryColumn!");
                }
            }
            else if (selectedColumn is CustomColumnInfo customInfo)
            {
                DataEditorViewModel = new StringRuleDataEditorViewModel();
            }
            else
                throw new InvalidOperationException("Unsupported column type!");
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Public methods -----------------------------------------------------

        public RuleEditorViewModel(List<BaseColumnInfo> availableColumns)
        {
            AvailableColumns = new ObservableCollection<BaseColumnInfo>(availableColumns);
            SelectedColumn = AvailableColumns.First();
            Foreground = Colors.Black;
            Background = Colors.Transparent;
        }

        // Public properties --------------------------------------------------

        public ObservableCollection<BaseColumnInfo> AvailableColumns { get; }

        public BaseColumnInfo SelectedColumn
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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
