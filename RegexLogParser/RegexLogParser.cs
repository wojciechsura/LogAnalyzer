using LogAnalyzer.API.LogParser;
using LogAnalyzer.API.Models;
using LogAnalyzer.API.Types;
using RegexLogParser.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RegexLogParser
{
    class RegexLogParser : ILogParser
    {
        private class LogEntryBuilder
        {
            public LogEntry Build()
            {
                return new LogEntry(Date, Severity, Message);
            }

            public DateTime Date { get; set; } = DateTime.MinValue;
            public string Severity { get; set; } = null;
            public string Message { get; set; } = null;
        }

        private readonly RegexLogParserConfiguration configuration;
        private readonly List<Action<string, LogEntryBuilder>> groupActions;
        private Regex regex;

        private void BuildGroupActions()
        {
            for (int i = 0; i < configuration.GroupDefinitions.Count; i++)
            {
                var definition = configuration.GroupDefinitions[i];
                if (definition is DateGroupDefinition dateGroupDefinition)
                {
                    groupActions.Add((value, logEntryBuilder) =>
                    {
                        try
                        {
                            logEntryBuilder.Date = DateTime.ParseExact(value, dateGroupDefinition.Format, CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            logEntryBuilder.Date = DateTime.MinValue;
                        }
                    });
                }
                else if (definition is SeverityGroupDefinition severityGroupDefinition)
                {
                    groupActions.Add((value, logEntryBuilder) =>
                    {
                        logEntryBuilder.Severity = value;
                    });
                }
                else if (definition is MessageGroupDefinition messageGroupDefinition)
                {
                    groupActions.Add((value, logEntryBuilder) =>
                    {
                        logEntryBuilder.Message = value;
                    });
                }
                else if (definition is CustomGroupDefinition customGroupDefinition)
                {
                    throw new InvalidOperationException("Custom groups are not yet supported!");
                }
            }
        }

        public RegexLogParser(RegexLogParserConfiguration configuration)
        {
            this.configuration = configuration;
            regex = new Regex(configuration.Regex);

            groupActions = new List<Action<string, LogEntryBuilder>>();
            BuildGroupActions();
        }

        public List<BaseColumnInfo> GetColumnInfos()
        {
            var result = new List<BaseColumnInfo>();

            for (int i = 0; i < configuration.GroupDefinitions.Count; i++)
            {
                var definition = configuration.GroupDefinitions[i];
                if (definition.GetColumn() != LogEntryColumn.Custom)
                    result.Add(new CommonColumnInfo(definition.GetColumn()));
                else
                    throw new InvalidOperationException("Custom columns are not yet supported!");
            }

            return result;
        }

        public (LogEntry, ParserOperation) Parse(string line, LogEntry lastEntry)
        {
            Match match = regex.Match(line);
            if (match.Success)
            {
                LogEntryBuilder builder = new LogEntryBuilder();

                for (int i = 1; i < Math.Min(match.Groups.Count, groupActions.Count + 1); i++)
                {
                    groupActions[i - 1].Invoke(match.Groups[i].Value, builder);
                }

                var entry = builder.Build();

                return (entry, ParserOperation.AddNew);
            }
            else
            {
                if (lastEntry != null)
                {
                    var newLastEntry = new LogEntry(lastEntry.Date,
                        lastEntry.Severity,
                        lastEntry.Message + "\n" + line);

                    return (newLastEntry, ParserOperation.ReplaceLast);
                }
                else
                {
                    return (null, ParserOperation.None);
                }
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
