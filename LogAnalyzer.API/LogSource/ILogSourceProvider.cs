﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.API.LogSource
{
    public interface ILogSourceProvider
    {
        ILogSourceEditorViewModel CreateEditorViewModel();
        string UniqueName { get; }
    }
}
