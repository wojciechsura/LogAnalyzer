using LogAnalyzer.API.Models;
using LogAnalyzer.Models.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.Views.FilterConfigWindow
{
    public class FilterConfigModel
    {
        public FilterConfigModel(FilterConfig currentConfig, List<BaseColumnInfo> currentColumns)
        {
            CurrentConfig = currentConfig;
            CurrentColumns = currentColumns;
        }

        public FilterConfig CurrentConfig { get; }
        public List<BaseColumnInfo> CurrentColumns { get; }
    }
}
