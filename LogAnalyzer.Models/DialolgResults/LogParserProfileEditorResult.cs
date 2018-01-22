using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Services.Models.DialolgResults
{
    public class LogParserProfileEditorResult
    {
        public LogParserProfileEditorResult(Guid guid)
        {
            this.Guid = guid;
        }

        public Guid Guid { get; }
    }
}
