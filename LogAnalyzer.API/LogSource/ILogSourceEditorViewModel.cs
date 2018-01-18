﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.API.LogSource
{
    public interface ILogSourceEditorViewModel
    {
        ILogSourceConfiguration BuildConfiguration();
        bool Validate();

        string DisplayName { get; }
        string EditorResource { get; }
    }
}