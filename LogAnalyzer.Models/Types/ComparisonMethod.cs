using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.Types
{
    public enum ComparisonMethod
    {
        [Description("Less than")]
        LessThan,
        [Description("Less than or equal")]
        LessThanOrEqual,
        [Description("Equal")]
        Equal,
        [Description("More than or equal")]
        MoreThanOrEqual,
        [Description("More than")]
        MoreThan,
        [Description("Contains")]
        Contains,
        [Description("Not contains")]
        NotContains,
        [Description("Matches")]
        Matches,
        [Description("Not matches")]
        NotMatches
    }
}
