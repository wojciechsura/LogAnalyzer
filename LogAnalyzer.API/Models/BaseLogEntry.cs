﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.API.Models
{
    public class BaseLogEntry
    {
        public BaseLogEntry(DateTime date, string severity, string message, IReadOnlyList<string> customFields)
        {
            Date = date;
            Severity = severity;
            Message = message;
            CustomFields = customFields;
        }

        public DateTime Date { get; }
        public string DisplayDate => Date.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
        public string Severity { get; }
        public string Message { get; }
        public IReadOnlyList<string> CustomFields { get; }
    }
}
