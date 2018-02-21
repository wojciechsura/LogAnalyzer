using LogAnalyzer.API.Models;
using LogAnalyzer.Common.Tools;
using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Export.Services
{
    class ExportService : IExportService
    {
        public string ExportToHtml(IList<LogRecord> records, List<BaseColumnInfo> columns)
        {
            if (records == null)
                throw new ArgumentNullException(nameof(records));

            StringBuilder builder = new StringBuilder();

            using (HtmlTag.Open(builder, "html"))
            using (HtmlTag.Open(builder, "body"))
            {
                using (HtmlTag.Open(builder, "table", "border=\"1\""))
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
                        using (HtmlTag.Open(builder, "tr"))
                        {
                            for (int col = 0; col < columns.Count; col++)
                            {
                                using (HtmlTag.Open(builder, "td"))
                                {
                                    builder.Append(columns[col].GetStringValue(records[i].LogEntry));
                                }
                            }
                        }
                    }
                }
            }

            return builder.ToString();
        }
    }
}
