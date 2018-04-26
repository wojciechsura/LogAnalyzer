using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.Views.JsonCodeWindow
{
    public class JsonCodeModel
    {
        public string Code { get; set; }
        public bool ShowCancel { get; set; }
        public string Title { get; set; }
        public string Hint { get; set; }
    }
}
