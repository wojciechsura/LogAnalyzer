using LogAnalyzer.API.Models;
using LogAnalyzer.Engine.Infrastructure.Predicates;
using LogAnalyzer.Models.Engine;
using LogAnalyzer.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Infrastructure.Processing
{
    class LogFilteringConfig
    {
        public LogFilteringConfig(FilterConfig config, List<BaseColumnInfo> currentColumns)
        {
            DefaultAction = config.DefaultAction;

            List<Filter> filters = new List<Filter>();
            for (int i = 0; i < config.FilterEntries.Count; i++)
            {
                FilterEntry filterEntry = config.FilterEntries[i];
                Func<LogEntry, bool> predicate = PredicateBuilder.BuildPredicate(filterEntry.PredicateDescription, currentColumns);
                if (predicate != null)
                    filters.Add(new Filter(predicate, filterEntry.Action));
            }

            this.Filters = filters;
        }

        public FilterAction DefaultAction { get; }
        public IReadOnlyList<Filter> Filters { get; }
    }
}
