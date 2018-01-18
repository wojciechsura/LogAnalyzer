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
using LogAnalyzer.BusinessLogic.Infrastructure;
using LogAnalyzer.BusinessLogic.Models;
using LogAnalyzer.Wpf.Input;

namespace LogAnalyzer.BusinessLogic.ViewModels
{
    public class MainWindowViewModel
    {
        // Private fields -----------------------------------------------------

        private readonly IMainWindowAccess access;

        // Private methods ----------------------------------------------------

        private void DoOpen()
        {
            var result = new ModalDialogResult<OpenResult>();
            Func<IOpenWindowAccess, OpenWindowViewModel> factory = (access) => Container.Instance.Resolve<OpenWindowViewModel>(
                new ParameterOverride("access", access), 
                new ParameterOverride("result", result));
            access.ShowOpenDialog(factory);

            if (result.DialogResult)
            {
                // Create new document
            }
        }

        // Public methods -----------------------------------------------------

        public MainWindowViewModel(IMainWindowAccess access)
        {
            this.access = access;

            OpenCommand = new SimpleCommand((obj) => DoOpen());
        }

        // Public properties --------------------------------------------------

        public ICommand OpenCommand { get; private set; }
    }
}
