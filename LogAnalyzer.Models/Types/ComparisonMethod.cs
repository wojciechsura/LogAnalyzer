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
        [Description("Contains")]
        [SummaryDisplay("containing")]
        Contains,
        [Description("Not contains")]
        [SummaryDisplay("not containing")]
        NotContains,
        [Description("Matches")]
        [SummaryDisplay("matching regular expression")]
        Matches,
        [Description("Not matches")]
        [SummaryDisplay("not matching regular expression")]
        NotMatches,
        [Description("Less than")]
        [SummaryDisplay("less than")]
        LessThan,
        [Description("Less than or equal")]
        [SummaryDisplay("less or equal to")]
        LessThanOrEqual,
        [Description("Equal")]
        [SummaryDisplay("equal to")]
        Equal,
        [Description("More than or equal")]
        [SummaryDisplay("more or equal to")]
        MoreThanOrEqual,
        [Description("More than")]
        [SummaryDisplay("more than")]
        MoreThan
    }
}
