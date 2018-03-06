using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.Windows;
using LogAnalyzer.Services.Common;
using LogAnalyzer.Models.DialogResults;
using LogAnalyzer.Models.Engine;
using LogAnalyzer.Models.Views.HighlightConfigWindow;
using LogAnalyzer.Models.Views.FilterConfigWindow;
using LogAnalyzer.Models.Views.FindWindow;
using LogAnalyzer.Models.Views.OpenWindow;
using LogAnalyzer.Models.Views.JumpToTime;
using LogAnalyzer.Models.Views.ColumnSelectionWindow;
using LogAnalyzer.Models.Views.NoteWindow;
using LogAnalyzer.Models.Views.LogMessageVisualizerWindow;
using LogAnalyzer.Scripting;
using LogAnalyzer.Models.Views.ProcessingProfileNameWindow;

namespace LogAnalyzer.Services
{
    public class DialogService : IDialogService
    {
        private PythonEditorWindow pythonEditorWindow = null;

        public ModalDialogResult<HighlightConfig> ConfigHighlighting(HighlightConfigModel model)
        {
            HighlightConfigWindow window = new HighlightConfigWindow(model);
            window.ShowDialog();
            return window.DataResult;
        }

        public ModalDialogResult<FilterConfig> ConfigFiltering(FilterConfigModel model)
        {
            FilterConfigWindow window = new FilterConfigWindow(model);
            window.ShowDialog();
            return window.DataResult;
        }

        public ModalDialogResult<LogParserProfileEditorResult> EditLogParserProfile(Guid guid, List<string> sampleLines)
        {
            ParserProfileEditorWindow editorWindow = new ParserProfileEditorWindow(guid, sampleLines);
            editorWindow.ShowDialog();
            return editorWindow.DataResult;
        }

        public ModalDialogResult<LogParserProfileEditorResult> NewLogParserProfile(List<string> sampleLines)
        {
            ParserProfileEditorWindow editorWindow = new ParserProfileEditorWindow(Guid.Empty, sampleLines);
            editorWindow.ShowDialog();
            return editorWindow.DataResult;
        }

        public ModalDialogResult<OpenResult> OpenLog(BaseOpenFilesModel model)
        {
            OpenWindow openWindow = new OpenWindow(model);
            openWindow.ShowDialog();
            return openWindow.DataResult;
        }

        public ModalDialogResult<SearchConfig> OpenFind(FindModel model)
        {
            FindWindow findWindow = new FindWindow(model);
            findWindow.ShowDialog();
            return findWindow.DataResult;
        }

        public ModalDialogResult<JumpToTimeResult> OpenJumpToTime(JumpToTimeModel model)
        {
            JumpToTimeWindow jumpToTimeWindow = new JumpToTimeWindow(model);
            jumpToTimeWindow.ShowDialog();
            return jumpToTimeWindow.DataResult;
        }

        public ModalDialogResult<ColumnSelectionResult> SelectColumn(ColumnSelectionModel model)
        {
            ColumnSelectionWindow window = new ColumnSelectionWindow(model);
            window.ShowDialog();
            return window.DataResult;
        }

        public ModalDialogResult<NoteResult> EditAnnotations(NoteModel model)
        {
            NoteWindow window = new NoteWindow(model);
            window.ShowDialog();
            return window.DataResult;
        }

        public void VisualizeMessage(LogMessageVisualizerModel model)
        {
            LogMessageVisualizerWindow window = new LogMessageVisualizerWindow(model);
            window.ShowDialog();
        }

        public void OpenConfiguration()
        {
            ConfigurationWindow window = new ConfigurationWindow();
            window.ShowDialog();
        }

        public void OpenPythonEditor()
        {
            if (pythonEditorWindow == null)
                pythonEditorWindow = new PythonEditorWindow();

            pythonEditorWindow.Show();
        }

        public void OpenLicesneWindow()
        {
            LicenseWindow window = new LicenseWindow();
            window.ShowDialog();
        }

        public ModalDialogResult<ProcessingProfileNameResult> ChooseProfileName(ProcessingProfileNameModel model)
        {
            ProcessingProfileNameWindow window = new ProcessingProfileNameWindow(model);
            window.ShowDialog();
            return window.DataResult;
        }
    }
}
