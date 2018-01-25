using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.BusinessLogic.ViewModels.Interfaces
{
    public interface IOpenWindowAccess
    {
        void Close(bool dialogResult);
    }
}
