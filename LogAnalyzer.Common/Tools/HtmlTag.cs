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
        private readonly bool lineBreak;

        private HtmlTag(StringBuilder builder, string tag, string attributes, bool lineBreak)
        {
            this.builder = builder;
            this.tag = tag;
            this.lineBreak = lineBreak;

            if (lineBreak)
                builder.AppendLine($"<{tag}{(attributes != null ? " " + attributes : "")}>");
            else
                builder.Append($"<{tag}{(attributes != null ? " " + attributes : "")}>");
        }

        public static HtmlTag Open(StringBuilder builder, string tag, string attributes = null, bool lineBreak = true)
        {
            return new HtmlTag(builder, tag, attributes, lineBreak);
        }

        public void Dispose()
        {
            if (lineBreak)
                builder.AppendLine($"</{tag}>");
            else
                builder.Append($"</{tag}>");
        }
    }
}
