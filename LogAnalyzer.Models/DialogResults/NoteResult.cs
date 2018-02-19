using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.DialogResults
{
    public class NoteResult
    {
        public NoteResult(string note)
        {
            Note = note;
        }

        public string Note { get; }
    }
}
