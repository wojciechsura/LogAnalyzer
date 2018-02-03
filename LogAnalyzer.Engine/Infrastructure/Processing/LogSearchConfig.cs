using LogAnalyzer.API.Models;
using LogAnalyzer.Engine.Infrastructure.Predicates;
using LogAnalyzer.Models.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Infrastructure.Processing
{
    class LogSearchConfig
    {
        public LogSearchConfig(SearchConfig config, List<BaseColumnInfo> availableColumns)
        {
            Predicate = PredicateBuilder.BuildPredicate(config.PredicateDescription, availableColumns);
        }

        public Func<LogEntry, bool> Predicate { get; }
    }
}
