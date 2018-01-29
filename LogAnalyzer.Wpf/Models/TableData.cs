using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Wpf.Models
{
    public class TableData
    {
        public TableData(List<string> columnHeaders, List<TableDataRow> rows)
        {
            for (int i = 0; i < rows.Count; i++)
                if (rows[i].Cells.Count != columnHeaders.Count)
                    throw new ArgumentException(nameof(rows));

            ColumnHeaders = columnHeaders;
            Rows = rows;
        }

        public List<string> ColumnHeaders { get; }
        public List<TableDataRow> Rows { get; }
    }
}
