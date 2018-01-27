using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Infrastructure.Events
{
    class LastParsedEntriesItemReplacedEvent : BaseEvent
    {
        private readonly int index;

        public LastParsedEntriesItemReplacedEvent(int index)
        {
            this.index = index;
        }

        public override string ToString()
        {
            return $"[P]-[ ]-[ ] Last parsed entries item replaced - index: {Index}";
        }

        public int Index => index;
    }
}
