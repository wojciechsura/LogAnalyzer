using LogAnalyzer.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.Views.ColumnSelectionWindow
{
    public class ColumnSelectionModel
    {
        public ColumnSelectionModel(List<BaseColumnInfo> currentColumns)
        {
            CurrentColumns = currentColumns;
        }

        public List<BaseColumnInfo> CurrentColumns { get; }
    }
}
