using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.Services.WinApiService
{
    public class FilterDefinition
    {
        public FilterDefinition(string filter, string text)
        {
            Filter = filter;
            Text = text;
        }

        public string Filter { get; private set; }
        public string Text { get; private set; }
    }
}
