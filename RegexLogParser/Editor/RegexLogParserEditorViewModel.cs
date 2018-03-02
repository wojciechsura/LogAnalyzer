using LogAnalyzer.API.LogParser;
using LogAnalyzer.API.Models;
using LogAnalyzer.API.Types;
using LogAnalyzer.Services.Interfaces;
using LogAnalyzer.Wpf.Input;
using LogAnalyzer.Wpf.Models;
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
using System.Windows.Input;

namespace RegexLogParser.Editor
{
    class RegexLogParserEditorViewModel : ILogParserEditorViewModel, INotifyPropertyChanged
    {
        // Private constants --------------------------------------------------

        private readonly string DISPLAY_NAME = "Regular expression parser";
        private readonly string EDITOR_RESOURCE = "RegexLogEditorTemplate";

        // Private fields -----------------------------------------------------

        private readonly IMessagingService messagingService;
        
        private string regularExpression;
        private GroupDefinitionViewModel selectedGroupDefinition;
        private int selectedGroupDefinitionIndex;
        private readonly ObservableCollection<GroupDefinitionViewModel> groupDefinitions;

        private readonly Condition itemSelectedCondition;
        private readonly Condition firstItemSelectedCondition;
        private readonly Condition lastItemSelectedCondition;

        private TableData resultData;
        private string sampleData;

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

        private List<BaseLogEntry> ParseSampleData(RegexLogParser parser)
        {
            List<BaseLogEntry> result = new List<BaseLogEntry>();

            string[] sampleLines = Regex.Split(SampleData, "\r\n|\r|\n");

            BaseLogEntry lastEntry = null;
            for (int i = 0; i < sampleLines.Length; i++)
            {
                (BaseLogEntry entry, ParserOperation operation) = parser.Parse(sampleLines[i], lastEntry);
                switch (operation)
                {
                    case ParserOperation.AddNew:
                        {
                            if (entry == null)
                                throw new InvalidOperationException("Entry cannot be null if operation is AddNew!");

                            result.Add(entry);
                            lastEntry = entry;
                            break;
                        }
                    case ParserOperation.ReplaceLast:
                        {
                            result[result.Count - 1] = entry ?? throw new InvalidOperationException("Entry cannot be null if operation is AddNew!");
                            lastEntry = entry;
                            break;
                        }
                    case ParserOperation.None:
                        {
                            if (entry != null)
                                throw new InvalidOperationException("Entry must be null if operation is None!");

                            break;
                        }
                        throw new InvalidOperationException("Unsupported parser operation!");
                }
            }

            return result;
        }

        private List<TableDataRow> BuildTableDataRows(List<BaseLogEntry> result, List<BaseColumnInfo> columnInfos)
        {
            List<TableDataRow> rows = new List<TableDataRow>();
            for (int i = 0; i < result.Count; i++)
            {
                BaseLogEntry entry = result[i];

                List<string> rowData = new List<string>();

                for (int j = 0; j < columnInfos.Count; j++)
                {
                    if (columnInfos[j] is CommonColumnInfo commonInfo)
                    {
                        switch (commonInfo.Column)
                        {
                            case LogEntryColumn.Date:
                                {
                                    rowData.Add(entry.Date.ToString());
                                    break;
                                }
                            case LogEntryColumn.Message:
                                {
                                    rowData.Add(entry.Message);
                                    break;
                                }
                            case LogEntryColumn.Severity:
                                {
                                    rowData.Add(entry.Severity);
                                    break;
                                }
                            case LogEntryColumn.Custom:
                                throw new InvalidOperationException("Critical error: custom column in CommonColumnInfo");
                            default:
                                throw new InvalidOperationException("Unsupported column!");
                        }
                    }
                    else if (columnInfos[j] is CustomColumnInfo customInfo)
                    {
                        if (customInfo.Index < entry.CustomFields.Count)
                            rowData.Add(entry.CustomFields[customInfo.Index]);
                        else
                            rowData.Add(null);
                    }
                }

                rows.Add(new TableDataRow(rowData));
            }

            return rows;
        }

        private List<string> BuildTableDataColumns(List<BaseColumnInfo> columnInfos)
        {
            List<string> columns = new List<string>();
            for (int i = 0; i < columnInfos.Count; i++)
            {
                columns.Add(columnInfos[i].Header);                
            }

            return columns;
        }

        private TableData BuildTableData(RegexLogParser parser, List<BaseLogEntry> result)
        {
            var columnInfos = parser.GetColumnInfos();

            List<string> columns = BuildTableDataColumns(columnInfos);
            List<TableDataRow> rows = BuildTableDataRows(result, columnInfos);

            TableData data = new TableData(columns, rows);
            return data;
        }

        private void DoTestParser()
        {
            ValidationResult validationResult = Validate();
            if (!validationResult.Valid)
            {
                messagingService.Warn(validationResult.Message);
                return;
            }

            if (String.IsNullOrEmpty(SampleData))
            {
                messagingService.Warn("Enter sample data first!");
                return;
            }

            // Try parse data
            var configuration = (RegexLogParserConfiguration)GetConfiguration();
            var parser = new RegexLogParser(configuration);

            List<BaseLogEntry> result = ParseSampleData(parser);
            TableData data = BuildTableData(parser, result);

            ResultData = data;
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Public methods -----------------------------------------------------

        public RegexLogParserEditorViewModel(ILogParserProvider parentProvider, IMessagingService messagingService)
        {
            this.Provider = parentProvider;
            this.messagingService = messagingService;

            groupDefinitions = new ObservableCollection<GroupDefinitionViewModel>();

            itemSelectedCondition = new Condition(false);
            firstItemSelectedCondition = new Condition(false);
            lastItemSelectedCondition = new Condition(false);

            AddGroupDefinitionCommand = new SimpleCommand((obj) => DoAddGroupDefinition());
            RemoveGroupDefinitionCommand = new SimpleCommand((obj) => DoRemoveGroupDefinition(), itemSelectedCondition);
            MoveLeftCommand = new SimpleCommand((obj) => DoMoveLeft(), !firstItemSelectedCondition & itemSelectedCondition);
            MoveRightCommand = new SimpleCommand((obj) => DoMoveRight(), !lastItemSelectedCondition & itemSelectedCondition);
            TestParserCommand = new SimpleCommand((obj) => DoTestParser());
        }

        public ILogParserConfiguration GetConfiguration()
        {
            if (!Validate().Valid)
                throw new InvalidOperationException("Cannot get configuration, editor values are not valid!");

            var groupDefinitions = new List<BaseGroupDefinition>();
            for (int i = 0; i < this.groupDefinitions.Count; i++)
            {
                BaseGroupDefinition data = this.groupDefinitions[i].GetGroupDefinition();
                groupDefinitions.Add(data);
            }

            var configuration = new RegexLogParserConfiguration
            {
                Regex = regularExpression,
                GroupDefinitions = groupDefinitions
            };


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

            // Unique group types (except custom)
            var groupTypes = groupDefinitions
                .Where(d => d.SelectedGroupType.Column != LogEntryColumn.Custom)
                .Select(g => g.SelectedGroupType.Column)
                .ToList();
            if (groupTypes.Count() != groupTypes.Distinct().Count())
                return new ValidationResult(false, "Common (non-custom) group types cannot be used more than once!");

            for (int i = 0; i < groupDefinitions.Count; i++)
            {
                ValidationResult result = groupDefinitions[i].Validate();
                if (!result.Valid)
                    return result;
            }

            return new ValidationResult(true, null);
        }

        public void SetSampleLines(List<string> sampleLines)
        {
            SampleData = String.Join("\n", sampleLines);
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
                itemSelectedCondition.Value = (value >= 0);
                firstItemSelectedCondition.Value = (value == 0);
                lastItemSelectedCondition.Value = (value == groupDefinitions.Count - 1);
                OnPropertyChanged(nameof(SelectedGroupDefinitionIndex));
            }
        }

        public string SampleData
        {
            get => sampleData;
            set
            {
                sampleData = value;
                OnPropertyChanged(nameof(SampleData));
            }
        }

        public ICommand AddGroupDefinitionCommand { get; }
        public ICommand RemoveGroupDefinitionCommand { get; }
        public ICommand MoveLeftCommand { get; }
        public ICommand MoveRightCommand { get; }
        public ICommand TestParserCommand { get; }

        public TableData ResultData
        {
            get => resultData;
            set
            {
                resultData = value;
                OnPropertyChanged(nameof(ResultData));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
