﻿using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.Windows;
using LogAnalyzer.Services.Common;
using LogAnalyzer.Models.DialogResults;

namespace LogAnalyzer.Services
{
    public class DialogService : IDialogService
    {
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
    }
}
