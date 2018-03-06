using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.DialogResults
{
    public class ProcessingProfileNameResult
    {
        public ProcessingProfileNameResult(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
