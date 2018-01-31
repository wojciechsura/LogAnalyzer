using LogAnalyzer.Models.Engine.PredicateDescriptions;
using System.Windows.Media;

namespace LogAnalyzer.Models.Engine
{
    public class HighlightEntry
    {
        public HighlightEntry()
        {
            
        }

        public PredicateDescription Condition { get; set; }
        public Color Foreground { get; set; }
        public Color Background { get; set; }
    }
}