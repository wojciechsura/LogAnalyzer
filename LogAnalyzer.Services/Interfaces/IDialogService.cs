﻿using System;
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
using LogAnalyzer.Models.Views.NoteWindow;
using LogAnalyzer.Models.Views.LogMessageVisualizerWindow;
using LogAnalyzer.Models.Views.ProcessingProfileNameWindow;
using LogAnalyzer.Models.Views.ScriptNameWindow;
using LogAnalyzer.Models.Views.JsonCodeWindow;
using LogAnalyzer.Scripting;

namespace LogAnalyzer.Services.Interfaces
{
    public interface IDialogService
    {
        ModalDialogResult<OpenResult> OpenLog(BaseOpenFilesModel files);
        ModalDialogResult<LogParserProfileEditorResult> EditLogParserProfile(Guid guid, List<string> sampleLines);
        ModalDialogResult<LogParserProfileEditorResult> NewLogParserProfile(List<string> sampleLines);
        ModalDialogResult<HighlightConfig> ConfigHighlighting(HighlightConfigModel model);
        ModalDialogResult<FilterConfig> ConfigFiltering(FilterConfigModel model);
        ModalDialogResult<SearchConfig> OpenFind(FindModel model);
        ModalDialogResult<JumpToTimeResult> OpenJumpToTime(JumpToTimeModel model);
        ModalDialogResult<ColumnSelectionResult> SelectColumn(ColumnSelectionModel model);
        ModalDialogResult<NoteResult> EditAnnotations(NoteModel model);
        ModalDialogResult<JsonCodeResult> OpenJsonCodeWindow(JsonCodeModel model);
        void VisualizeMessage(LogMessageVisualizerModel model);
        void OpenConfiguration();
        void OpenPythonEditor(IScriptingHost scriptingHost);
        ModalDialogResult<ProcessingProfileNameResult> ChooseProfileName(ProcessingProfileNameModel model);
        ModalDialogResult<ScriptNameResult> ChooseScriptName(ScriptNameModel model);
    }
}
