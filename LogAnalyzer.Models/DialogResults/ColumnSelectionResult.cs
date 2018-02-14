using LogAnalyzer.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.DialogResults
{
    public class ColumnSelectionResult
    {
        public ColumnSelectionResult(BaseColumnInfo selectedColumn)
        {
            SelectedColumn = selectedColumn;
        }

        public BaseColumnInfo SelectedColumn { get; }
    }
}
