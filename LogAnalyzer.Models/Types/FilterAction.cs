using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.Types
{
    public enum FilterAction
    {
        [Description("Include")]
        [SummaryDisplay("included")]
        Include,
        [Description("Exclude")]
        [SummaryDisplay("excluded")]
        Exclude
    }
}
