using LogAnalyzer.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LogAnalyzer.Engine.Infrastructure.Processing
{
    class Highlighter
    {
        public Highlighter(Func<LogEntry, bool> predicate, Color foreground, Color background)
        {
            Predicate = predicate;
            Foreground = foreground;
            Background = background;
        }

        public Func<LogEntry, bool> Predicate { get; }
        public Color Foreground { get; }
        public Color Background { get; }
    }
}
