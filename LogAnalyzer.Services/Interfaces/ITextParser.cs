﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Services.Interfaces
{
    public interface ITextParser
    {
        string ParseToHtmlPage(string logMessage);
    }
}
