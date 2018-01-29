using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.API.Types;
using LogAnalyzer.API.Types.Attributes;
using LogAnalyzer.Common.Extensions;
using RegexLogParser.Editor.GroupConfiguration;

namespace RegexLogParser.Editor
{
    class GroupDefinitionViewModel : INotifyPropertyChanged
    {
        public class GroupDisplayInfo
        {
            public GroupDisplayInfo(LogEntryColumn column, string name)
            {
                this.Column = column;
                DisplayName = name;
            }

            public LogEntryColumn Column { get; }
            public string DisplayName { get; }
        }

        private string displayName;
        private BaseGroupConfigurationViewModel groupConfiguration;
        private readonly List<GroupDisplayInfo> availableColumnTypes;
        private GroupDisplayInfo selectedColumnType;

        private void HandleSelectedColumnTypeChanged(GroupDisplayInfo value)
        {
            DisplayName = value.DisplayName;
            switch (value.Column)
            {
                case LogEntryColumn.Date:
                    {
                        GroupConfiguration = new DateGroupConfigurationViewModel();
                        break;
                    }
                case LogEntryColumn.Custom:
                    {
                        GroupConfiguration = new CustomGroupConfigurationViewModel();
                        break;
                    }
                default:
                    {
                        GroupConfiguration = null;
                        break;
                    }
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public GroupDefinitionViewModel()
        {
            availableColumnTypes = new List<GroupDisplayInfo>();
            foreach (LogEntryColumn column in Enum.GetValues(typeof(LogEntryColumn)))
            {
                availableColumnTypes.Add(new GroupDisplayInfo(column,
                    column.GetAttribute<ColumnHeaderAttribute>().Header));
            }

            SelectedColumnType = availableColumnTypes.First();
        }

        public string DisplayName
        {
            get
            {
                return displayName;
            }
            set
            {
                displayName = value;
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        public List<GroupDisplayInfo> AvailableColumns => availableColumnTypes;
        public GroupDisplayInfo SelectedColumnType
        {
            get
            {
                return selectedColumnType;
            }
            set
            {
                selectedColumnType = value;
                HandleSelectedColumnTypeChanged(value);
                OnPropertyChanged(nameof(SelectedColumnType));
            }
        }

        public BaseGroupConfigurationViewModel GroupConfiguration
        {
            get
            {
                return groupConfiguration;
            }
            set
            {
                groupConfiguration = value;
                OnPropertyChanged(nameof(groupConfiguration));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
