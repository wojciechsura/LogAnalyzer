using LogAnalyzer.Models.Engine.ProcessConditions;
using System.Windows.Media;

namespace LogAnalyzer.Models.Engine
{
    public class HighlightEntry
    {
        public HighlightEntry(ProcessCondition condition, Color foreground, Color background)
        {
            Condition = condition;
            Foreground = foreground;
            Background = background;
        }

        public ProcessCondition Condition { get; }
        public Color Foreground { get; }
        public Color Background { get; }
    }
}