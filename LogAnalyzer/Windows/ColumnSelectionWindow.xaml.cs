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
using LogAnalyzer.Models.DialogResults;
using LogAnalyzer.Services.Common;
using LogAnalyzer.Models.Views.ColumnSelectionWindow;
using Unity;
using Unity.Resolution;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using LogAnalyzer.BusinessLogic.ViewModels;

namespace LogAnalyzer.Windows
{
    /// <summary>
    /// Interaction logic for ColumnSelectionWindow.xaml
    /// </summary>
    public partial class ColumnSelectionWindow : Window, IColumnSelectionWindowAccess
    {
        private ColumnSelectionWindowViewModel viewModel;

        public ColumnSelectionWindow(ColumnSelectionModel model)
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<ColumnSelectionWindowViewModel>(new ParameterOverride("access", this), new ParameterOverride("model", model));
            DataContext = viewModel;
        }

        public ModalDialogResult<ColumnSelectionResult> DataResult => viewModel.Result;

        public void Close(bool result)
        {
            DialogResult = result;
        }
    }
}
