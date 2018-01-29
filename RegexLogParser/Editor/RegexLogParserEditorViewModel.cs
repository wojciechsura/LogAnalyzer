using LogAnalyzer.API.LogParser;
using LogAnalyzer.Wpf.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
        private GroupDefinitionViewModel selectedColumnDefinition;
        private int selectedColumnDefinitionIndex;
        private readonly ObservableCollection<GroupDefinitionViewModel> columnDefinitions;

        private readonly Condition itemSelectedCondition;
        private readonly Condition firstItemSelectedCondition;
        private readonly Condition lastItemSelectedCondition;

        // Private methods ----------------------------------------------------

        private void DoAddColumnDefinition()
        {
            var newColumnDefinition = new GroupDefinitionViewModel();
            columnDefinitions.Add(newColumnDefinition);
            SelectedColumnDefinition = newColumnDefinition;
        }

        private void DoRemoveColumnDefinition()
        {
            columnDefinitions.Remove(selectedColumnDefinition);
            SelectedColumnDefinition = null;
        }

        private void DoMoveLeft()
        {
            int index = columnDefinitions.IndexOf(selectedColumnDefinition);
            columnDefinitions.Move(index, index - 1);
        }

        private void DoMoveRight()
        {
            int index = columnDefinitions.IndexOf(selectedColumnDefinition);
            columnDefinitions.Move(index, index + 1);
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

            columnDefinitions = new ObservableCollection<GroupDefinitionViewModel>();

            itemSelectedCondition = new Condition(false);
            firstItemSelectedCondition = new Condition(false);
            lastItemSelectedCondition = new Condition(false);

            AddColumnDefinition = new SimpleCommand((obj) => DoAddColumnDefinition());
            RemoveColumnDefinition = new SimpleCommand((obj) => DoRemoveColumnDefinition(), itemSelectedCondition);
            MoveLeft = new SimpleCommand((obj) => DoMoveLeft(), !firstItemSelectedCondition & itemSelectedCondition);
            MoveRight = new SimpleCommand((obj) => DoMoveRight(), !lastItemSelectedCondition & itemSelectedCondition);
        }

        public ILogParserConfiguration GetConfiguration()
        {
            var configuration = new RegexLogParserConfiguration();
            configuration.Regex = regularExpression;

            return configuration;
        }

        public void SetConfiguration(ILogParserConfiguration configuration)
        {
            var regexConfig = configuration as RegexLogParserConfiguration;
            if (regexConfig == null)
                throw new ArgumentNullException(nameof(configuration));

            regularExpression = regexConfig.Regex;
        }

        public bool Validate()
        {
            throw new NotImplementedException();
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

        public ObservableCollection<GroupDefinitionViewModel> ColumnDefinitions => columnDefinitions;

        public GroupDefinitionViewModel SelectedColumnDefinition
        {
            get
            {
                return selectedColumnDefinition;
            }
            set
            {
                selectedColumnDefinition = value;
                OnPropertyChanged(nameof(SelectedColumnDefinition));
            }
        }

        public int SelectedColumnDefinitionIndex
        {
            get
            {
                return selectedColumnDefinitionIndex;
            }
            set
            {
                selectedColumnDefinitionIndex = value;
                itemSelectedCondition.Value = (value != -1);
                firstItemSelectedCondition.Value = (value == 0);
                lastItemSelectedCondition.Value = (value == columnDefinitions.Count - 1);
                OnPropertyChanged(nameof(SelectedColumnDefinitionIndex));
            }
        }

        public SimpleCommand AddColumnDefinition { get; }
        public SimpleCommand RemoveColumnDefinition { get; }
        public SimpleCommand MoveLeft { get; }
        public SimpleCommand MoveRight { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
