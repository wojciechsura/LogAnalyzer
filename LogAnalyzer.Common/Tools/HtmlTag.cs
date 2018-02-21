using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Common.Tools
{
    public class HtmlTag : IDisposable
    {
        private readonly StringBuilder builder;
        private readonly string tag;
        private readonly bool lineBreakAfterOpen;
        private readonly bool lineBreakAfterClose;

        private HtmlTag(StringBuilder builder, string tag, string attributes, bool lineBreakAfterOpen, bool lineBreakAfterClose)
        {
            this.builder = builder;
            this.tag = tag;
            this.lineBreakAfterOpen = lineBreakAfterOpen;
            this.lineBreakAfterClose = lineBreakAfterClose;

            if (lineBreakAfterOpen)
                builder.AppendLine($"<{tag}{(attributes != null ? " " + attributes : "")}>");
            else
                builder.Append($"<{tag}{(attributes != null ? " " + attributes : "")}>");
        }

        public static HtmlTag Open(StringBuilder builder, string tag, string attributes = null, bool lineBreakAfterOpen = false, bool lineBreakAfterClose = true)
        {
            return new HtmlTag(builder, tag, attributes, lineBreakAfterOpen, lineBreakAfterClose);
        }

        public void Dispose()
        {
            if (lineBreakAfterClose)
                builder.AppendLine($"</{tag}>");
            else
                builder.Append($"</{tag}>");
        }
    }
}
