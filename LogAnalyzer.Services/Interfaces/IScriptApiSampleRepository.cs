﻿using LogAnalyzer.Models.ApiSamples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Services.Interfaces
{
    public interface IScriptApiSampleRepository 
    {
        IReadOnlyList<ApiSampleModel> ApiSamples { get; }
    }
}
