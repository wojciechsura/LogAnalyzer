using LogAnalyzer.BusinessLogic.ViewModels;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using LogAnalyzer.Models.Engine;
using LogAnalyzer.Models.Views.FilterConfigWindow;
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
using Unity;
using Unity.Resolution;

namespace LogAnalyzer.Windows
{
    /// <summary>
    /// Interaction logic for FilterConfigWindow.xaml
    /// </summary>
    public partial class FilterConfigWindow : Window, IFilterConfigWindowAccess
    {
        private readonly FilterConfigWindowViewModel viewModel;

        public FilterConfigWindow()
        {
            InitializeComponent();
        }

        public FilterConfigWindow(FilterConfigModel model)
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<FilterConfigWindowViewModel>(new ParameterOverride("access", this), new ParameterOverride("model", model));
            DataContext = viewModel;
        }

        public void Close(bool result)
        {
            DialogResult = result;
        }

        public ModalDialogResult<FilterConfig> DataResult => viewModel.Result;
    }
}
