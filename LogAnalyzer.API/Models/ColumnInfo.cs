using LogAnalyzer.API.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.API.Models
{
    public class ColumnInfo
    {
        public ColumnInfo(LogEntryColumn column)
        {
            Column = column;
            Index = null;
        }

        public ColumnInfo(LogEntryColumn column, int? index)
        {
            Column = column;
            Index = index;
        }

        public LogEntryColumn Column { get; }
        public int? Index { get; }
    }
}
