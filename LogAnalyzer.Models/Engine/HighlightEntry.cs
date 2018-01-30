using LogAnalyzer.Models.Engine.ProcessConditions;
using System.Windows.Media;

namespace LogAnalyzer.Models.Engine
{
    public class HighlightEntry
    {
        public HighlightEntry()
        {
            
        }

        public ProcessCondition Condition { get; set; }
        public Color Foreground { get; set; }
        public Color Background { get; set; }
    }
}