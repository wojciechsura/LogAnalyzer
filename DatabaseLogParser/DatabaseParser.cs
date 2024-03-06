using DatabaseLogParser.Configuration;
using LogAnalyzer.API.LogParser;
using LogAnalyzer.API.Models;
using LogAnalyzer.API.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DatabaseLogParser
{
    internal class DatabaseParser : ILogParser
    {
        private DatabaseLogParserConfiguration configuration;

        private string[] SplitUnquoted(string s, char separator)
        {
            bool quote = false;

            int last = 0;
            int i = 0;

            List<string> result = new List<string>();

            while (i < s.Length)
            {
                if (s[i] == '\"')
                {
                    quote = !quote;
                    i++;
                    continue;
                }

                if (s[i] == '\\' && quote)
                {
                    i++;
                    if (i >= s.Length)
                        throw new InvalidOperationException("Invalid escape sequence!");
                    i++;
                    continue;
                }

                if (s[i] == separator && !quote)
                {
                    string splitted = s.Substring(last, i - last);
                    result.Add(splitted);

                    i++;
                    last = i;
                    continue;
                }

                i++;
            }

            if (quote)
                throw new InvalidOperationException("Unterminated quote!");

            string lastSplitted = s.Substring(last);
            result.Add(lastSplitted);

            return result.ToArray();
        }

        private string Unescape(string s)
        {
            StringBuilder sb = new StringBuilder();

            int i = 1;

            while (i < s.Length - 1)
            {
                if (s[i] == '\\')
                {
                    i++;
                    if (i >= s.Length)
                        throw new InvalidOperationException("Invalid escaped string!");
                }

                sb.Append(s[i]);
                i++;
            }

            return sb.ToString();
        }

        public DatabaseParser(DatabaseLogParserConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Dispose()
        {
            
        }

        public List<BaseColumnInfo> GetColumnInfos()
        {
            var result = new List<BaseColumnInfo>();
            int customColumns = 0;

            for (int i = 0; i < configuration.FieldDefinitions.Count; i++)
            {
                var definition = configuration.FieldDefinitions[i];
                if (definition.GetColumn() != LogEntryColumn.Custom)
                    result.Add(new CommonColumnInfo(definition.GetColumn()));
                else
                    result.Add(new CustomColumnInfo(customColumns++, ((CustomFieldDefinition)definition).Name));
            }

            return result;
        }

        public (BaseLogEntry, ParserOperation) Parse(string line, BaseLogEntry lastEntry)
        {
            var entries = SplitUnquoted(line, ',')
                .Select(e => SplitUnquoted(e, ':'))
                .Select(arr => new { Column = arr[0], Value = Unescape(arr[1]) })
                .ToList();

            DateTime date = DateTime.MinValue;
            string message = string.Empty;
            string severity = string.Empty;
            List<string> customFields = new List<string>();

            for (int i = 0; i < configuration.FieldDefinitions.Count; i++)
            {
                var fieldDefinition = configuration.FieldDefinitions[i];

                var field = entries.FirstOrDefault(e => e.Column == fieldDefinition.Field)

                if (fieldDefinition is DateFieldDefinition dateField)
                {
                    if (field == null || !DateTime.TryParseExact(field.Value, dateField.Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                        date = DateTime.MinValue;
                }
                else if (fieldDefinition is CustomFieldDefinition customField)
                {
                    if (field != null)
                        customFields.Add(field.Value);
                    else
                        customFields.Add(string.Empty);
                }
                else if (fieldDefinition is MessageFieldDefinition)
                {
                    message = field?.Value ?? string.Empty;
                }
                else if (fieldDefinition is SeverityFieldDefinition)
                {
                    severity = field?.Value ?? string.Empty;
                }
            }

            var newEntry = new BaseLogEntry(date, severity, message, customFields);

            return (newEntry, ParserOperation.AddNew);
        }
    }
}