using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.API.Types;
using LogAnalyzer.API.Types.Attributes;
using LogAnalyzer.Common.Extensions;
using RegexLogParser.Configuration;
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
        private readonly List<GroupDisplayInfo> availableGroupTypes;
        private GroupDisplayInfo selectedGroupType;

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
                case LogEntryColumn.Message:
                    {
                        GroupConfiguration = new MessageGroupConfigurationViewModel();
                        break;
                    }
                case LogEntryColumn.Severity:
                    {
                        GroupConfiguration = new SeverityGroupConfigurationViewModel();
                        break;
                    }
                default:
                    throw new InvalidOperationException("Not recognized log entry column!");
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public GroupDefinitionViewModel()
        {
            availableGroupTypes = new List<GroupDisplayInfo>();
            foreach (LogEntryColumn column in Enum.GetValues(typeof(LogEntryColumn)))
            {
                availableGroupTypes.Add(new GroupDisplayInfo(column,
                    column.GetAttribute<ColumnHeaderAttribute>().Header));
            }

            SelectedGroupType = availableGroupTypes.First();
        }

        public GroupDefinitionViewModel(BaseGroupDefinition groupDefinition)
        {
            groupConfiguration = BaseGroupConfigurationViewModel.FromGroupDefinition(groupDefinition);
            selectedGroupType = availableGroupTypes.Single(a => groupDefinition.GetColumn() == a.Column);

            OnPropertyChanged(nameof(GroupConfiguration));
            OnPropertyChanged(nameof(SelectedGroupType));            
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

        public List<GroupDisplayInfo> AvailableGroups => availableGroupTypes;

        public GroupDisplayInfo SelectedGroupType
        {
            get
            {
                return selectedGroupType;
            }
            set
            {
                selectedGroupType = value;
                HandleSelectedColumnTypeChanged(value);
                OnPropertyChanged(nameof(SelectedGroupType));
            }
        }

        public BaseGroupDefinition GetGroupDefinition()
        {
            return groupConfiguration.GetGroupDefinition();
        }

        public ValidationResult Validate()
        {
            return groupConfiguration.Validate();
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
