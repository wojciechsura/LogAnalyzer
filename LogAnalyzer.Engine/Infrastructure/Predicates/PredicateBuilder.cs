using LogAnalyzer.API.Models;
using LogAnalyzer.API.Types;
using LogAnalyzer.Models.Engine.PredicateDescriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

                    break;
                case Models.Types.ComparisonMethod.LessThanOrEqual:
                    break;
                case Models.Types.ComparisonMethod.Equal:
                    break;
                case Models.Types.ComparisonMethod.MoreThanOrEqual:
                    break;
                case Models.Types.ComparisonMethod.MoreThan:
                    break;
                case Models.Types.ComparisonMethod.Contains:
                    break;
                case Models.Types.ComparisonMethod.NotContains:
                    break;
                case Models.Types.ComparisonMethod.Matches:
                    break;
                case Models.Types.ComparisonMethod.NotMatches:
                    break;
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
            throw new NotImplementedException();
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
