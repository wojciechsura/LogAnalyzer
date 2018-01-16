using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LogAnalyzer.API.LogSource
{
    interface ILogEditor
    {
        ILogEditorViewModel ViewModel { get; }
        Control EditorControl { get; }
    }
}
