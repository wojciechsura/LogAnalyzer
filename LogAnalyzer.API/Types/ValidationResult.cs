using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.API.Types
{
    public class ValidationResult
    {
        public ValidationResult(bool valid, string message)
        {
            Valid = valid;
            Message = message;
        }

        public bool Valid { get; set; }
        public string Message { get; set; }
    }
}
