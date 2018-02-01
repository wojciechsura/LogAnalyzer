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

        public int Index => index;
    }
}
