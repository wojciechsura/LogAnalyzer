using System.Collections.Generic;

namespace LogAnalyzer.Wpf.Models
{
    public class TableDataRow
    {
        public TableDataRow(List<string> cells)
        {
            Cells = cells;
        }

        public List<string> Cells { get; }
    }
}