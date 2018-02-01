using LogAnalyzer.API.Models;
using LogAnalyzer.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Infrastructure.Processing
{
    class Filter
    {
        public Filter(Func<LogEntry, bool> predicate, FilterAction action)
        {
            Predicate = predicate;
            Action = action;
        }

        public Func<LogEntry, bool> Predicate { get; }
        public FilterAction Action { get; }
    }
}
