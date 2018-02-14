using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.Models.DialogResults;
using LogAnalyzer.Models.Engine;
using LogAnalyzer.Models.Views.FilterConfigWindow;
using LogAnalyzer.Models.Views.FindWindow;
using LogAnalyzer.Models.Views.HighlightConfigWindow;
using LogAnalyzer.Models.Views.OpenWindow;
using LogAnalyzer.Services.Common;
using LogAnalyzer.Models.Views.JumpToTime;
using LogAnalyzer.Models.Views.ColumnSelectionWindow;

namespace LogAnalyzer.Services.Interfaces
{
    public interface IDialogService
    {
        ModalDialogResult<OpenResult> OpenLog(OpenFilesModel files);
        ModalDialogResult<LogParserProfileEditorResult> EditLogParserProfile(Guid guid);
        ModalDialogResult<LogParserProfileEditorResult> NewLogParserProfile();
        ModalDialogResult<HighlightConfig> ConfigHighlighting(HighlightConfigModel model);
        ModalDialogResult<FilterConfig> ConfigFiltering(FilterConfigModel model);
        ModalDialogResult<SearchConfig> OpenFind(FindModel model);
        ModalDialogResult<JumpToTimeResult> OpenJumpToTime(JumpToTimeModel model);
        ModalDialogResult<ColumnSelectionResult> SelectColumn(ColumnSelectionModel model);
    }
}
