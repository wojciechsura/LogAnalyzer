using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.DialogResults
{
    public class JsonCodeResult
    {
        public JsonCodeResult(string code)
        {
            Code = code;
        }

        public string Code { get; }
    }
}
