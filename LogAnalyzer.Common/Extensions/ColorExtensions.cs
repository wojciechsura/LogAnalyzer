using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LogAnalyzer.Common.Extensions
{
    public static class ColorExtensions
    {
        public static string ToHtmlColor(this Color color)
        {
            return String.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", color.R, color.G, color.B, color.A);
        }
    }
}
