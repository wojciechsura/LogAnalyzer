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
using Autofac;
using LogAnalyzer.BusinessLogic.ViewModels;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using LogAnalyzer.Models.Engine;
using LogAnalyzer.Models.Views.HighlightConfigWindow;
using LogAnalyzer.Services.Common;

namespace LogAnalyzer.Windows
{
    /// <summary>
    /// Interaction logic for HighlightConfigWindow.xaml
    /// </summary>
    public partial class HighlightConfigWindow : Window, IHighlightConfigWindowAccess
    {
        private readonly HighlightConfigWindowViewModel viewModel;

        public HighlightConfigWindow(HighlightConfigModel model)
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<HighlightConfigWindowViewModel>(new NamedParameter("access", this), new NamedParameter("model", model));
            DataContext = viewModel;
        }

        public void Close(bool result)
        {
            DialogResult = result;
        }

        public ModalDialogResult<HighlightConfig> DataResult => viewModel.Result;
    }
}
