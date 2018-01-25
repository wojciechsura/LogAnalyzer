using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.Models.DialogResults;
using LogAnalyzer.Services.Common;

namespace LogAnalyzer.Services.Interfaces
{
    public interface IDialogService
    {
        ModalDialogResult<OpenResult> OpenLog();
        ModalDialogResult<LogParserProfileEditorResult> EditLogParserProfile(Guid guid);
        ModalDialogResult<LogParserProfileEditorResult> NewLogParserProfile();
    }
}
