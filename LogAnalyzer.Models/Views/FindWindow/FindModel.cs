using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.Views.FindWindow
{
    public class FindModel
    {
        public FindModel(List<string> availableCustomColumns)
        {
            AvailableCustomColumns = availableCustomColumns;
        }

        public List<string> AvailableCustomColumns { get; }
    }
}
