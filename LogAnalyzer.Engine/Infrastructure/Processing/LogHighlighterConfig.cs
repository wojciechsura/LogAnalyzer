using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.API.Models;
using LogAnalyzer.Engine.Infrastructure.Predicates;
using LogAnalyzer.Models.Engine;

namespace LogAnalyzer.Engine.Infrastructure.Processing
{
    class LogHighlighterConfig
    {
        public LogHighlighterConfig(HighlightConfig filter, List<BaseColumnInfo> currentColumns)
        {
            List<Highlighter> highlighters = new List<Highlighter>();
            for (int i = 0; i < filter.HighlightEntries.Count; i++)
            {
                HighlightEntry highlightEntry = filter.HighlightEntries[i];
                Func<LogEntry, bool> predicate = PredicateBuilder.BuildPredicate(highlightEntry.PredicateDescription, currentColumns);
                if (predicate != null)
                    highlighters.Add(new Highlighter(predicate, highlightEntry.Foreground, highlightEntry.Background));
            }

            this.Highlighters = highlighters;
        }

        public IReadOnlyList<Highlighter> Highlighters { get; }
    }
}
