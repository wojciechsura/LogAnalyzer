using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.Services.Models;
using LogAnalyzer.Windows;

namespace LogAnalyzer.Services
{
    public class DialogService : IDialogService
    {
        public ModalDialogResult<OpenResult> OpenLog()
        {
            OpenWindow openWindow = new OpenWindow();
            openWindow.ShowDialog();
            return openWindow.DataResult;
        }
    }
}
