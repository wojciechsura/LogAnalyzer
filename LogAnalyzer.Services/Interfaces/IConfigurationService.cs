﻿using LogAnalyzer.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Services.Interfaces
{
    public interface IConfigurationService
    {
        AppConfiguration Configuration { get; }
    }
}
