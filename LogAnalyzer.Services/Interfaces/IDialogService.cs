using LogAnalyzer.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.Services.Models.DialolgResults;
using LogAnalyzer.Services.Common;
using LogAnalyzer.Services.Models.DialogResults;

namespace LogAnalyzer.Services.Interfaces
{
    public interface IDialogService
    {
        ModalDialogResult<OpenResult> OpenLog();
        ModalDialogResult<LogParserProfileEditorResult> EditLogParserProfile(Guid guid);
        ModalDialogResult<LogParserProfileEditorResult> NewLogParserProfile();
    }
}
