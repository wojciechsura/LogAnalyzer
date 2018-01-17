using LogAnalyzer.BusinessLogic.Infrastructure;
using LogAnalyzer.BusinessLogic.Models;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LogAnalyzer.BusinessLogic.ViewModels
{
    public class OpenWindowViewModel
    {
        private readonly IOpenWindowAccess access;
        private readonly ModalDialogResult<OpenResult> result;

        public OpenWindowViewModel(IOpenWindowAccess access, ModalDialogResult<OpenResult> result)
        {
            this.access = access;
            this.result = result;
        }
    }
}
