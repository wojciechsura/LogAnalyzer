using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Infrastructure.Events
{
    class AddedNewParsedEntriesEvent : BaseEvent
    {
        public AddedNewParsedEntriesEvent(int start, int count)
        {
            Start = start;
            Count = count;
        }

        public override string ToString()
        {
            return $"[P]-[ ]-[ ] Added new parsed entries - start: {Start}, count: {Count}";
        }

        public int Start { get; }
        public int Count { get; }
    }
}
