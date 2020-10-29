using Spooksoft.VisualStateManager.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Scripting
{
    public interface IScriptingHost
    {
        void Run(string script);
        BaseCondition CanRunCondition { get; }
    }
}
