using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.DialogResults
{
    public class ScriptNameResult
    {
        public ScriptNameResult(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
