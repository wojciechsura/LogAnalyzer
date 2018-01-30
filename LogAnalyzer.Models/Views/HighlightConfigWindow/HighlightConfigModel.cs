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
        public HighlightConfigModel(HighlightConfig currentConfig, List<BaseColumnInfo> currentColumns)
        {
            CurrentConfig = currentConfig;
            CurrentColumns = currentColumns;
        }

        public HighlightConfig CurrentConfig { get; }
        public List<BaseColumnInfo> CurrentColumns { get; }
    }
}
