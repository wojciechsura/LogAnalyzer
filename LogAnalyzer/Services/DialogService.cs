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

namespace LogAnalyzer.Services
{
    public class DialogService : IDialogService
    {
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

        public ModalDialogResult<LogParserProfileEditorResult> EditLogParserProfile(Guid guid)
        {
            ParserProfileEditorWindow editorWindow = new ParserProfileEditorWindow(guid);
            editorWindow.ShowDialog();
            return editorWindow.DataResult;
        }

        public ModalDialogResult<LogParserProfileEditorResult> NewLogParserProfile()
        {
            ParserProfileEditorWindow editorWindow = new ParserProfileEditorWindow(Guid.Empty);
            editorWindow.ShowDialog();
            return editorWindow.DataResult;
        }

        public ModalDialogResult<OpenResult> OpenLog()
        {
            OpenWindow openWindow = new OpenWindow();
            openWindow.ShowDialog();
            return openWindow.DataResult;
        }

        public ModalDialogResult<FindResult> OpenFind(FindModel model)
        {
            FindWindow findWindow = new FindWindow(model);
            findWindow.ShowDialog();
            return findWindow.DataResult;
        }
    }
}
