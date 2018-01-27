using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LogAnalyzer.Models.Engine
{
    public class HighlightInfo
    {
        public HighlightInfo(Color foreground, Color background)
        {
            Foreground = foreground;
            Background = background;
        }

        public Color Foreground { get; }
        public Color Background { get; }
    }
}
