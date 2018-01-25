using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;
using Unity.Resolution;
using LogAnalyzer.Dependencies;
using LogAnalyzer.Wpf.Input;
using LogAnalyzer.Services.Interfaces;

namespace LogAnalyzer.BusinessLogic.ViewModels
{
    public class MainWindowViewModel
    {
        // Private fields -----------------------------------------------------

        private readonly IMainWindowAccess access;
        private readonly IDialogService dialogService;

        private readonly IEngineFactory engineFactory;

        private IEngine engine;

        // Private methods ----------------------------------------------------

        private void DoOpen()
        {
            var result = dialogService.OpenLog();

            if (result.DialogResult)
            {
                // TODO      
            }
        }

        // Public methods -----------------------------------------------------

        public MainWindowViewModel(IMainWindowAccess access, IDialogService dialogService, IEngineFactory engineFactory)
        {
            this.access = access;
            this.dialogService = dialogService;
            this.engineFactory = engineFactory;

            OpenCommand = new SimpleCommand((obj) => DoOpen());
        }

        // Public properties --------------------------------------------------

        public ICommand OpenCommand { get; private set; }
    }
}
