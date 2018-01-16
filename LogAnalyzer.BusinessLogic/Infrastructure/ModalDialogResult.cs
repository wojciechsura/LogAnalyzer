using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.BusinessLogic.Infrastructure
{
    public class ModalDialogResult<T>
        where T : class
    {
        public bool DialogResult { get; set; } = false;
        public T Result { get; set; } = null;
    }
}
