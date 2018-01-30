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
using LogAnalyzer.BusinessLogic.ViewModels;
using LogAnalyzer.Models.Engine;
using LogAnalyzer.Models.Views.HighlightConfigWindow;
using LogAnalyzer.Services.Common;

namespace LogAnalyzer.Windows
{
    /// <summary>
    /// Interaction logic for HighlightConfigWindow.xaml
    /// </summary>
    public partial class HighlightConfigWindow : Window
    {
        private readonly HighlightConfigWindowViewModel viewModel;

        public HighlightConfigWindow(HighlightConfigModel model)
        {
            InitializeComponent();

            viewModel = new HighlightConfigWindowViewModel(model);
            DataContext = viewModel;
        }

        public ModalDialogResult<HighlightConfig> DataResult => viewModel.Result;
    }
}
