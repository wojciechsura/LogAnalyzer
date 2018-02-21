using LogAnalyzer.API.Models;
using LogAnalyzer.Common.Extensions;
using LogAnalyzer.Common.Tools;
using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

            if (logRecord.LogEntry.HasNote)
            {
                using (HtmlTag.Open(builder, "tr", $"class=\"{(style != null ? style + " " : "")}\"single-row"))
                using (HtmlTag.Open(builder, "td", $"colspan=\"{columns.Count}\""))
                {
                    builder.Append("<i class=\"note\"></i>");
                    using (HtmlTag.Open(builder, "span", "class=\"single-row-content\""))
                    {
                        builder.Append(logRecord.LogEntry.Note);
                    }
                }
            }

            if (logRecord.LogEntry.IsProfilingPoint)
            {
                using (HtmlTag.Open(builder, "tr", $"class=\"{(style != null ? style + " " : "")}\"single-row"))
                using (HtmlTag.Open(builder, "td", $"colspan=\"{columns.Count}\""))
                {
                    builder.Append("<i class=\"stopwatch\"></i>");
                    using (HtmlTag.Open(builder, "span", "class=\"content\""))
                    {
                        builder.Append("Step ")
                            .Append(logRecord.LogEntry.ProfilingStep)
                            .Append(". Since previous: ");
                        using (HtmlTag.Open(builder, "span", "class=\"emphasize\""))
                            builder.Append(logRecord.LogEntry.TimeSpanFromPrevious);

                        builder.Append("Elapsed: ");

                        using (HtmlTag.Open(builder, "span", "class=\"emphasize\""))
                            builder.Append(logRecord.LogEntry.TimeSpanFromStart);
                    }
                }
            }
        }

        private void BuildTable(IList<LogRecord> records, List<BaseColumnInfo> columns, StringBuilder builder, Action<LogRecord, List<BaseColumnInfo>, StringBuilder> buildRowAction)
        {
            using (HtmlTag.Open(builder, "table", "class=\"logs\""))
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
            {
                using (HtmlTag.Open(builder, "Head"))
                using (HtmlTag.Open(builder, "Style", "type=\"text/css\""))
                {
                    string css = ResourceReader.ReadEmbeddedResource(Assembly.GetExecutingAssembly(), "LogAnalyzer.Export.Resources.BasicStyles.css");
                    builder.AppendLine(css);
                }

                using (HtmlTag.Open(builder, "body"))
                {
                    BuildTable(records, columns, builder, (record, cols, b) => BuildSimpleRow(record, cols, b));
                }
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
                        string css = ResourceReader.ReadEmbeddedResource(Assembly.GetExecutingAssembly(), "LogAnalyzer.Export.Resources.BasicStyles.css");
                        builder.AppendLine(css);

                        foreach (var kvp in styles)
                        {
                            builder.AppendLine($".{kvp.Value} {{")
                                .AppendLine($"    color: {kvp.Key.Foreground.ToHtmlColor()};")
                                .AppendLine($"    background-color: {kvp.Key.Background.ToHtmlColor()};")
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
