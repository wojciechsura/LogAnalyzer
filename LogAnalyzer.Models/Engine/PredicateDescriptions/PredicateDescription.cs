using LogAnalyzer.API.Models;
using LogAnalyzer.Models.Types;

namespace LogAnalyzer.Models.Engine.PredicateDescriptions
{
    public abstract class PredicateDescription
    {
        public ComparisonMethod Comparison { get; set; }
    }
}