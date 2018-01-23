using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LogAnalyzer.Models.Engine
{
    public class LogHighlighting
    {
        private readonly Color foreground;
        private readonly Color background;

        public LogHighlighting(Color foreground, Color background)
        {
            this.foreground = foreground;
            this.background = background;
        }

        public Color Foreground => foreground;

        public Color Background => background;
    }
}
