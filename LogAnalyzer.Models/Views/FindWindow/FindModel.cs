using LogAnalyzer.Models.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.Views.FindWindow
{
    public class FindModel
    {
        public FindModel(SearchConfig searchConfig, List<string> availableCustomColumns)
        {
            SearchConfig = searchConfig;
            AvailableCustomColumns = availableCustomColumns;
        }

        public SearchConfig SearchConfig { get; }
        public List<string> AvailableCustomColumns { get; }
    }
}
