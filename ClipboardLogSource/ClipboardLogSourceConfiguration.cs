﻿using LogAnalyzer.API.LogSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipboardLogSource
{
    public class ClipboardLogSourceConfiguration : ILogSourceConfiguration
    {
        public ClipboardLogSourceConfiguration(string filename)
        {
            Filename = filename;
        }

        public string Filename { get; private set; }
    }
}
