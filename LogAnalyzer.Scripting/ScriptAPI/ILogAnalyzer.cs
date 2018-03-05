using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Scripting.ScriptAPI
{
    public interface ILogAnalyzer
    {
        void WriteLog(string message);
        void WritelnLog(string message);
    }
}
