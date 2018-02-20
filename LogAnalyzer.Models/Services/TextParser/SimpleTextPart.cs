using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.Services.TextParser
{
    public class SimpleTextPart : BaseTextPart
    {
        public SimpleTextPart(string text)
        {

        }

        public string Text { get; }
    }
}
