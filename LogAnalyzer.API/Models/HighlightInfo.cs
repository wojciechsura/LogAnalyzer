using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace LogAnalyzer.API.Models
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

        public static Color DefaultForeground => SystemColors.ControlTextColor;
        public static Color DefaultBackground => Colors.Transparent;
    }
}
