﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Engine.Infrastructure.Events
{
    class HighlightedItemsClearedEvent : BaseEvent
    {
        public override string ToString()
        {
            return "[ ]-[ ]-[H] Highlighted items cleared!";
        }
    }
}
