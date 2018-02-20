using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using LogAnalyzer.Models.Views.LogMessageVisualizerWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.BusinessLogic.ViewModels
{
    public class LogMessageVisualizerWindowViewModel
    {
        private readonly ILogMessageVisualizerWindowAccess access;
        private readonly LogMessageVisualizerModel model;

        public LogMessageVisualizerWindowViewModel(ILogMessageVisualizerWindowAccess access, LogMessageVisualizerModel model)
        {
            this.access = access;
            this.model = model;

            access.Display(model.Html);
        }
    }
}
