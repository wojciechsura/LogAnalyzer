using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.Services.Models;
using LogAnalyzer.Windows;
using LogAnalyzer.Services.Common;
using LogAnalyzer.Services.Models.DialogResults;
using LogAnalyzer.Services.Models.DialolgResults;

namespace LogAnalyzer.Services
{
    public class DialogService : IDialogService
    {
        public ModalDialogResult<LogParserProfileEditorResult> EditLogParserProfile(Guid guid)
        {
            throw new NotImplementedException();
        }

        public ModalDialogResult<LogParserProfileEditorResult> NewLogParserProfile()
        {
            throw new NotImplementedException();
        }

        public ModalDialogResult<OpenResult> OpenLog()
        {
            OpenWindow openWindow = new OpenWindow();
            openWindow.ShowDialog();
            return openWindow.DataResult;
        }
    }
}
