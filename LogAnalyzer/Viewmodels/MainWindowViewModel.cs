using LogAnalyzer.Viewmodels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Viewmodels
{
    public class MainWindowViewModel
    {
        private readonly IMainWindowAccess access;

        public MainWindowViewModel(IMainWindowAccess access)
        {
            this.access = access;
        }
    }
}
