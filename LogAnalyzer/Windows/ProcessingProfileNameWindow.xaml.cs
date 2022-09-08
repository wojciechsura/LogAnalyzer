using Autofac;
using LogAnalyzer.BusinessLogic.ViewModels;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using LogAnalyzer.Models.DialogResults;
using LogAnalyzer.Models.Views.ProcessingProfileNameWindow;
using LogAnalyzer.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LogAnalyzer.Windows
{
    /// <summary>
    /// Interaction logic for ProcessingProfileNameWindow.xaml
    /// </summary>
    public partial class ProcessingProfileNameWindow : Window, IProcessingProfileNameWindowAccess
    {
        private ProcessingProfileNameWindowViewModel viewModel;

        public ProcessingProfileNameWindow(ProcessingProfileNameModel model)
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<ProcessingProfileNameWindowViewModel>(new NamedParameter("access", this),
                new NamedParameter("model", model));
            DataContext = viewModel;
        }

        public ModalDialogResult<ProcessingProfileNameResult> DataResult => viewModel.Result;

        public void Close(bool result)
        {
            DialogResult = result;
        }
    }
}
