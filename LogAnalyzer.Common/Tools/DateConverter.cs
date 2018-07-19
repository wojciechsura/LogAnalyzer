using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Common.Tools
{
    public static class DateConverter
    {
        private static readonly string[] formats =
        {
            "yyyy-M-d H:m:s.fff",
            "yyyy-M-d H:m:s.ff",
            "yyyy-M-d H:m:s.f",
            "yyyy-M-d H:m:s",
            "yyyy-M-d H:m",
            "yyyy-M-d H",
            "yyyy.M.d H:m:s.fff",
            "yyyy.M.d H:m:s.ff",
            "yyyy.M.d H:m:s.f",
            "yyyy.M.d H:m:s",
            "yyyy.M.d H:m",
            "yyyy.M.d H",
            "M/d/yyyy HH:mm:ss.fff",
            "M/d/yyyy HH:mm:ss.ff",
            "M/d/yyyy HH:mm:ss.f",
            "M/d/yyyy HH:mm:s",
            "M/d/yyyy HH:m",
            "M/d/yyyy H",
        };

        public static DateTime ConvertToDate(string date)
        {
            foreach (var format in formats)
            {
                DateTime result;
                if (DateTime.TryParseExact(date, format, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out result))
                    return result;
            }

            throw new FormatException("Invalid date format!");
        }

        public static bool Validate(string date)
        {
            foreach (var format in formats)
            {
                try
                {
                    DateTime.ParseExact(date, format, CultureInfo.InvariantCulture);
                    return true;
                }
                catch
                {
                    // Try next
                }
            }

            return false;
        }
    }
}
