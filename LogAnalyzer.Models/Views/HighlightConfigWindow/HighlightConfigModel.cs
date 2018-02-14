using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.API.Models;
using LogAnalyzer.Models.Engine;

namespace LogAnalyzer.Models.Views.HighlightConfigWindow
{
    public class HighlightConfigModel
    {
        public HighlightConfigModel(HighlightConfig currentConfig, List<BaseColumnInfo> currentColumns, HighlightEntry newEntry = null)
        {
            CurrentConfig = currentConfig;
            CurrentColumns = currentColumns;
            NewEntry = newEntry;
        }

        public HighlightConfig CurrentConfig { get; }
        public List<BaseColumnInfo> CurrentColumns { get; }
        public HighlightEntry NewEntry { get; }
    }
}
