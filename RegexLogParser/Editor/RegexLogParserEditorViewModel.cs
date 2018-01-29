using LogAnalyzer.API.LogParser;
using LogAnalyzer.API.Types;
using LogAnalyzer.Wpf.Input;
using RegexLogParser.Configuration;
using RegexLogParser.Editor.GroupConfiguration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RegexLogParser.Editor
{
    class RegexLogParserEditorViewModel : ILogParserEditorViewModel, INotifyPropertyChanged
    {
        // Private constants --------------------------------------------------

        private readonly string DISPLAY_NAME = "Regular expression parser";
        private readonly string EDITOR_RESOURCE = "RegexLogEditorTemplate";

        // Private fields -----------------------------------------------------

        private string regularExpression;
        private GroupDefinitionViewModel selectedGroupDefinition;
        private int selectedGroupDefinitionIndex;
        private readonly ObservableCollection<GroupDefinitionViewModel> groupDefinitions;

        private readonly Condition itemSelectedCondition;
        private readonly Condition firstItemSelectedCondition;
        private readonly Condition lastItemSelectedCondition;

        // Private methods ----------------------------------------------------

        private void DoAddGroupDefinition()
        {
            var newGroupDefinition = new GroupDefinitionViewModel();
            groupDefinitions.Add(newGroupDefinition);
            SelectedGroupDefinition = newGroupDefinition;
        }

        private void DoRemoveGroupDefinition()
        {
            groupDefinitions.Remove(selectedGroupDefinition);
            SelectedGroupDefinition = null;
        }

        private void DoMoveLeft()
        {
            int index = groupDefinitions.IndexOf(selectedGroupDefinition);
            groupDefinitions.Move(index, index - 1);
        }

        private void DoMoveRight()
        {
            int index = groupDefinitions.IndexOf(selectedGroupDefinition);
            groupDefinitions.Move(index, index + 1);
        }

        private void Clear()
        {
            regularExpression = "";
            groupDefinitions.Clear();            
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Public methods -----------------------------------------------------

        public RegexLogParserEditorViewModel(ILogParserProvider parentProvider)
        {
            this.Provider = parentProvider;

            groupDefinitions = new ObservableCollection<GroupDefinitionViewModel>();

            itemSelectedCondition = new Condition(false);
            firstItemSelectedCondition = new Condition(false);
            lastItemSelectedCondition = new Condition(false);

            AddGroupDefinition = new SimpleCommand((obj) => DoAddGroupDefinition());
            RemoveGroupDefinition = new SimpleCommand((obj) => DoRemoveGroupDefinition(), itemSelectedCondition);
            MoveLeft = new SimpleCommand((obj) => DoMoveLeft(), !firstItemSelectedCondition & itemSelectedCondition);
            MoveRight = new SimpleCommand((obj) => DoMoveRight(), !lastItemSelectedCondition & itemSelectedCondition);
        }

        public ILogParserConfiguration GetConfiguration()
        {
            if (!Validate().Valid)
                throw new InvalidOperationException("Cannot get configuration, editor values are not valid!");

            var configuration = new RegexLogParserConfiguration();

            // Regular expression
            configuration.Regex = regularExpression;

            // Group definitions
            var groupDefinitions = new List<BaseGroupDefinition>();
            for (int i = 0; i < this.groupDefinitions.Count; i++)
            {
                BaseGroupDefinition data = this.groupDefinitions[i].GetGroupDefinition();
                groupDefinitions.Add(data);
            }
            configuration.GroupDefinitions = groupDefinitions;
            
            return configuration;
        }

        public void SetConfiguration(ILogParserConfiguration configuration)
        {
            var regexConfig = configuration as RegexLogParserConfiguration;
            if (regexConfig == null)
                throw new ArgumentNullException(nameof(configuration));

            Clear();

            // Regular expression
            regularExpression = regexConfig.Regex;

            for (int i = 0; i < regexConfig.GroupDefinitions.Count; i++)
            {
                GroupDefinitionViewModel groupDefinitionViewModel = new GroupDefinitionViewModel(regexConfig.GroupDefinitions[i]);
                groupDefinitions.Add(groupDefinitionViewModel);
            }
        }

        public ValidationResult Validate()
        {
            // Regular expression
            try
            {
                Regex regex = new Regex(regularExpression);
            }
            catch
            {
                return new ValidationResult(false, "Invalid regular expression!");
            }

            // Group count > 0
            if (groupDefinitions.Count == 0)
                return new ValidationResult(false, "You have to define at least one group definition!");

            // Unique custom group names
            var customGroupNames = groupDefinitions.Where(g => g.GroupConfiguration is CustomGroupConfigurationViewModel)
                    .Select(g => g.GroupConfiguration as CustomGroupConfigurationViewModel)
                    .Select(c => c.Name)
                    .ToList();
            if (customGroupNames.Count != customGroupNames.Distinct().Count())
                return new ValidationResult(false, "Custom group names must be unique!");

            // Unique group types
            var groupTypes = groupDefinitions
                .Select(g => g.SelectedGroupType.Column)
                .ToList();
            if (groupTypes.Count() != groupTypes.Distinct().Count())
                return new ValidationResult(false, "Group types cannot be used more than once!");

            for (int i = 0; i < groupDefinitions.Count; i++)
            {
                ValidationResult result = groupDefinitions[i].Validate();
                if (!result.Valid)
                    return result;
            }

            return new ValidationResult(true, null);
        }

        // Public properties --------------------------------------------------

        public ILogParserProvider Provider { get; }

        public string DisplayName => DISPLAY_NAME;

        public string EditorResource => EDITOR_RESOURCE;

        public string RegularExpression
        {
            get
            {
                return regularExpression;
            }
            set
            {
                regularExpression = value;
                OnPropertyChanged(nameof(RegularExpression));
            }
        }

        public ObservableCollection<GroupDefinitionViewModel> GroupDefinitions => groupDefinitions;

        public GroupDefinitionViewModel SelectedGroupDefinition
        {
            get
            {
                return selectedGroupDefinition;
            }
            set
            {
                selectedGroupDefinition = value;
                OnPropertyChanged(nameof(SelectedGroupDefinition));
            }
        }

        public int SelectedGroupDefinitionIndex
        {
            get
            {
                return selectedGroupDefinitionIndex;
            }
            set
            {
                selectedGroupDefinitionIndex = value;
                itemSelectedCondition.Value = (value != -1);
                firstItemSelectedCondition.Value = (value == 0);
                lastItemSelectedCondition.Value = (value == groupDefinitions.Count - 1);
                OnPropertyChanged(nameof(SelectedGroupDefinitionIndex));
            }
        }

        public SimpleCommand AddGroupDefinition { get; }
        public SimpleCommand RemoveGroupDefinition { get; }
        public SimpleCommand MoveLeft { get; }
        public SimpleCommand MoveRight { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
