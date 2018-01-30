using LogAnalyzer.API.Models;
using LogAnalyzer.Models.Types;

namespace LogAnalyzer.Models.Engine.ProcessConditions
{
    public abstract class ProcessCondition
    {
        public ComparisonMethod Comparison { get; set; }
    }
}