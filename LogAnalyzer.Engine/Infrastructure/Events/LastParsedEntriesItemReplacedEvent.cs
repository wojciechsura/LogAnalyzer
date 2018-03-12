using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Infrastructure.Events
{
    class LastParsedEntriesItemReplacedEvent : BaseEngineEvent
    {
        private readonly int metaIndex;

        public LastParsedEntriesItemReplacedEvent(int metaIndex)
        {
            this.metaIndex = metaIndex;
        }

        public int MetaIndex => metaIndex;
    }
}
