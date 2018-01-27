using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Infrastructure.Events
{
    class EntriesHighlightedEvent : BaseEvent
    {
        public EntriesHighlightedEvent(int start, int count)
        {
            Start = start;
            Count = count;
        }

        public override string ToString()
        {
            return $"[ ]-[ ]-[H] Log entries highlighted - start: {Start}, count: {Count}";
        }

        public int Start { get; }
        public int Count { get; }
    }
}
