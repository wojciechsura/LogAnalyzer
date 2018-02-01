using LogAnalyzer.API.Models;
using LogAnalyzer.API.Types;
using LogAnalyzer.Models.Engine.PredicateDescriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Infrastructure.Predicates
{
    public static class PredicateBuilder
    {
        private static Func<LogEntry, bool> BuildStringPredicate(Func<LogEntry, string> getStringFunc, StringPredicateDescription stringPredicateDesc)
        {
            switch (stringPredicateDesc.Comparison)
            {
                case Models.Types.ComparisonMethod.LessThan:
                    {
                        if (stringPredicateDesc.CaseSensitive)
                            return (logEntry) => getStringFunc(logEntry).CompareTo(stringPredicateDesc.Argument) < 0;
                        else
                            return (logEntry) => getStringFunc(logEntry).ToLowerInvariant().CompareTo(stringPredicateDesc.Argument.ToLowerInvariant()) < 0;
                    }                    
                case Models.Types.ComparisonMethod.LessThanOrEqual:
                    {
                        if (stringPredicateDesc.CaseSensitive)
                            return (logEntry) => getStringFunc(logEntry).CompareTo(stringPredicateDesc.Argument) <= 0;
                        else
                            return (logEntry) => getStringFunc(logEntry).ToLowerInvariant().CompareTo(stringPredicateDesc.Argument.ToLowerInvariant()) <= 0;
                    }
                case Models.Types.ComparisonMethod.Equal:
                    {
                        if (stringPredicateDesc.CaseSensitive)
                            return (logEntry) => getStringFunc(logEntry).CompareTo(stringPredicateDesc.Argument) == 0;
                        else
                            return (logEntry) => getStringFunc(logEntry).ToLowerInvariant().CompareTo(stringPredicateDesc.Argument.ToLowerInvariant()) == 0;
                    }
                case Models.Types.ComparisonMethod.MoreThanOrEqual:
                    {
                        if (stringPredicateDesc.CaseSensitive)
                            return (logEntry) => getStringFunc(logEntry).CompareTo(stringPredicateDesc.Argument) >= 0;
                        else
                            return (logEntry) => getStringFunc(logEntry).ToLowerInvariant().CompareTo(stringPredicateDesc.Argument.ToLowerInvariant()) >= 0;
                    }
                case Models.Types.ComparisonMethod.MoreThan:
                    {
                        if (stringPredicateDesc.CaseSensitive)
                            return (logEntry) => getStringFunc(logEntry).CompareTo(stringPredicateDesc.Argument) > 0;
                        else
                            return (logEntry) => getStringFunc(logEntry).ToLowerInvariant().CompareTo(stringPredicateDesc.Argument.ToLowerInvariant()) > 0;
                    }
                case Models.Types.ComparisonMethod.Contains:
                    {
                        if (stringPredicateDesc.CaseSensitive)
                            return (logEntry) => getStringFunc(logEntry).Contains(stringPredicateDesc.Argument);
                        else
                            return (logEntry) => getStringFunc(logEntry).ToLowerInvariant().Contains(stringPredicateDesc.Argument.ToLowerInvariant());
                    }
                case Models.Types.ComparisonMethod.NotContains:
                    if (stringPredicateDesc.CaseSensitive)
                        return (logEntry) => !(getStringFunc(logEntry).Contains(stringPredicateDesc.Argument));
                    else
                        return (logEntry) => !(getStringFunc(logEntry).ToLowerInvariant().Contains(stringPredicateDesc.Argument.ToLowerInvariant()));
                case Models.Types.ComparisonMethod.Matches:
                    {
                        Regex regex;
                        if (stringPredicateDesc.CaseSensitive)
                            regex = new Regex(stringPredicateDesc.Argument);
                        else
                            regex = new Regex(stringPredicateDesc.Argument, RegexOptions.IgnoreCase);

                        return (logEntry) => regex.IsMatch(getStringFunc(logEntry));
                    }
                case Models.Types.ComparisonMethod.NotMatches:
                    {
                        Regex regex;
                        if (stringPredicateDesc.CaseSensitive)
                            regex = new Regex(stringPredicateDesc.Argument);
                        else
                            regex = new Regex(stringPredicateDesc.Argument, RegexOptions.IgnoreCase);

                        return (logEntry) => !regex.IsMatch(getStringFunc(logEntry));
                    }
                default:
                    throw new ArgumentException("Not supported comparison type!");
            }

            return null;
        }

        private static Func<LogEntry, bool> BuildCustomPredicate(CustomPredicateDescription customPredicateDesc, List<BaseColumnInfo> currentColumns)
        {
            int index = -1;
            for (int i = 0; i < currentColumns.Count; i++)
            {
                if (currentColumns[i] is CustomColumnInfo customColumn)
                {
                    index++;
                    if (customColumn.Name == customPredicateDesc.Name)
                        break;
                }
            }

            if (index == -1)
                return null;
            else
                return BuildStringPredicate(entry => entry.CustomFields[index], customPredicateDesc);
        }

        private static Func<LogEntry, bool> BuildMessagePredicate(MessagePredicateDescription messagePredicateDesc, List<BaseColumnInfo> currentColumns)
        {
            if (!currentColumns
                .OfType<CommonColumnInfo>()
                .Any(c => c.Column == LogEntryColumn.Message))
                return null;

            return BuildStringPredicate(entry => entry.Message, messagePredicateDesc);
        }

        private static Func<LogEntry, bool> BuildSeverityPredicate(SeverityPredicateDescription severityPredicateDesc, List<BaseColumnInfo> currentColumns)
        {
            if (!currentColumns
                .OfType<CommonColumnInfo>()
                .Any(c => c.Column == LogEntryColumn.Severity))
                return null;

            return BuildStringPredicate(entry => entry.Severity, severityPredicateDesc);
        }

        private static Func<LogEntry, bool> BuildDatePredicate(DatePredicateDescription datePredicateDesc, List<BaseColumnInfo> currentColumns)
        {
            if (!currentColumns.OfType<CommonColumnInfo>().Any(cc => cc.Column == LogEntryColumn.Date))
                return null;

            switch (datePredicateDesc.Comparison)
            {
                case Models.Types.ComparisonMethod.LessThan:
                    {
                        return (logEntry) => logEntry.Date.CompareTo(datePredicateDesc.Argument) < 0;
                    }
                case Models.Types.ComparisonMethod.LessThanOrEqual:
                    {
                        return (logEntry) => logEntry.Date.CompareTo(datePredicateDesc.Argument) <= 0;
                    }
                case Models.Types.ComparisonMethod.Equal:
                    {
                        return (logEntry) => logEntry.Date.CompareTo(datePredicateDesc.Argument) == 0;
                    }
                case Models.Types.ComparisonMethod.MoreThanOrEqual:
                    {
                        return (logEntry) => logEntry.Date.CompareTo(datePredicateDesc.Argument) >= 0;
                    }
                case Models.Types.ComparisonMethod.MoreThan:
                    {
                        return (logEntry) => logEntry.Date.CompareTo(datePredicateDesc.Argument) > 0;
                    }
                default:
                    throw new ArgumentException("Invalid comparison method for date field!");
            }
        }

        public static Func<LogEntry, bool> BuildPredicate(PredicateDescription predicateDescription, List<BaseColumnInfo> currentColumns)
        {
            if (predicateDescription is DatePredicateDescription datePredicateDesc)
            {
                return BuildDatePredicate(datePredicateDesc, currentColumns);
            }
            else if (predicateDescription is SeverityPredicateDescription severityPredicateDesc)
            {
                return BuildSeverityPredicate(severityPredicateDesc, currentColumns);
            }
            else if (predicateDescription is MessagePredicateDescription messagePredicateDesc)
            {
                return BuildMessagePredicate(messagePredicateDesc, currentColumns);
            }
            else if (predicateDescription is CustomPredicateDescription customPredicateDesc)
            {
                return BuildCustomPredicate(customPredicateDesc, currentColumns);
            }
            else
                throw new ArgumentException("Invalid predicate description");
        }
    }
}
