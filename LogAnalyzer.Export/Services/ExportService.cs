﻿using LogAnalyzer.API.Models;
using LogAnalyzer.Common.Tools;
using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LogAnalyzer.Export.Services
{
    class ExportService : IExportService
    {
        private class HighlightStyle
        {
            public HighlightStyle(Color foreground, Color background)
            {
                Foreground = foreground;
                Background = background;
            }

            public override int GetHashCode()
            {
                return Foreground.GetHashCode() ^ Background.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj is HighlightStyle other)
                {
                    return other.Foreground == this.Foreground && other.Background == this.Background;
                }
                else
                    return false;
            }

            public bool IsDefault => Foreground == HighlightInfo.DefaultForeground && Background == HighlightInfo.DefaultBackground;

            public Color Foreground { get; }
            public Color Background { get; }
        }

        private void BuildSimpleRow(LogRecord logRecord, List<BaseColumnInfo> columns, StringBuilder builder)
        {
            using (HtmlTag.Open(builder, "tr"))
            {
                for (int col = 0; col < columns.Count; col++)
                {
                    using (HtmlTag.Open(builder, "td"))
                    {
                        builder.Append(columns[col].GetStringValue(logRecord.LogEntry));
                    }
                }
            }
        }

        private void BuildHighlightedRow(LogRecord logRecord, List<BaseColumnInfo> columns, StringBuilder builder, Dictionary<HighlightStyle, string> styles)
        {
            HighlightStyle currentHighlight = new HighlightStyle(logRecord.Highlight?.Foreground ?? HighlightInfo.DefaultForeground,
                logRecord.Highlight?.Background ?? HighlightInfo.DefaultBackground);

            string style = null;

            if (!currentHighlight.IsDefault)
            {
                if (styles.ContainsKey(currentHighlight))
                    style = styles[currentHighlight];
                else
                {
                    styles.Add(currentHighlight, $"style" + styles.Count());
                }
            }

            using (HtmlTag.Open(builder, "tr", style != null ? $"class=\"{style}\"" : ""))
            {
                for (int col = 0; col < columns.Count; col++)
                {
                    using (HtmlTag.Open(builder, "td"))
                    {
                        builder.Append(columns[col].GetStringValue(logRecord.LogEntry));
                    }
                }
            }

        }

        private void BuildTable(IList<LogRecord> records, List<BaseColumnInfo> columns, StringBuilder builder, Action<LogRecord, List<BaseColumnInfo>, StringBuilder> buildRowAction)
        {
            using (HtmlTag.Open(builder, "table"))
            {
                using (HtmlTag.Open(builder, "tr"))
                {
                    for (int col = 0; col < columns.Count; col++)
                    {
                        using (HtmlTag.Open(builder, "th"))
                        {
                            builder.Append(columns[col].Header);
                        }
                    }
                }

                for (int i = 0; i < records.Count; i++)
                {
                    buildRowAction(records[i], columns, builder);
                }
            }
        }

        public string ExportToHtml(IList<LogRecord> records, List<BaseColumnInfo> columns)
        {
            if (records == null)
                throw new ArgumentNullException(nameof(records));

            StringBuilder builder = new StringBuilder();

            using (HtmlTag.Open(builder, "html"))
            using (HtmlTag.Open(builder, "body"))
            {
                BuildTable(records, columns, builder, (record, cols, b) => BuildSimpleRow(record, cols, b));
            }

            return builder.ToString();
        }

        public string ExportToStyledHtml(IList<LogRecord> records, List<BaseColumnInfo> columns)
        {
            if (records == null)
                throw new ArgumentNullException(nameof(records));

            Dictionary<HighlightStyle, string> styles = new Dictionary<HighlightStyle, string>();

            StringBuilder tableBuilder = new StringBuilder();
            BuildTable(records, columns, tableBuilder, (record, cols, b) => BuildHighlightedRow(record, cols, b, styles));

            StringBuilder builder = new StringBuilder();
            using (HtmlTag.Open(builder, "html")) {
                using (HtmlTag.Open(builder, "head"))
                {
                    using (HtmlTag.Open(builder, "style", "type=\"text/css\""))
                    {
                        foreach (var kvp in styles)
                        {
                            builder.AppendLine($".{kvp.Value} {{")
                                .AppendLine($"    color: {string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", kvp.Key.Foreground.R, kvp.Key.Foreground.G, kvp.Key.Foreground.B, kvp.Key.Foreground.A)};")
                                .AppendLine($"    background-color: {string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", kvp.Key.Background.R, kvp.Key.Background.G, kvp.Key.Background.B, kvp.Key.Background.A)};")
                                .AppendLine("}");
                        }
                    }
                }

                using (HtmlTag.Open(builder, "body"))
                {
                    builder.AppendLine(tableBuilder.ToString());
                }
            }

            return builder.ToString();
        }
    }
}
