﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.Engine.ProcessConditions
{
    public abstract class StringProcessCondition : ProcessCondition
    {
        public string Argument { get; set; }
    }
}