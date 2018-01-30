using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.Models.DialogResults;
using LogAnalyzer.Models.Engine;
using LogAnalyzer.Models.Views.HighlightConfigWindow;
using LogAnalyzer.Services.Common;

namespace LogAnalyzer.Services.Interfaces
{
    public interface IDialogService
    {
        ModalDialogResult<OpenResult> OpenLog();
        ModalDialogResult<LogParserProfileEditorResult> EditLogParserProfile(Guid guid);
        ModalDialogResult<LogParserProfileEditorResult> NewLogParserProfile();
        ModalDialogResult<HighlightConfig> ConfigHighlighting(HighlightConfigModel model);
    }
}
