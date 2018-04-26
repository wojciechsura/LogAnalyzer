using LogAnalyzer.BusinessLogic.ViewModels;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using LogAnalyzer.Models.DialogResults;
using LogAnalyzer.Models.Views.JsonCodeWindow;
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
    /// Interaction logic for JsonCodeWindow.xaml
    /// </summary>
    public partial class JsonCodeWindow : Window, IJsonCodeWindowAccess
    {
        private JsonCodeWindowViewModel viewModel;

        private void tbCode_GotFocus(object sender, RoutedEventArgs e)
        {
            tbCode.SelectAll();
        }

        public JsonCodeWindow(JsonCodeModel model)
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<JsonCodeWindowViewModel>(new ParameterOverride("access", this), new ParameterOverride("model", model));
            DataContext = viewModel;
        }

        public void Close(bool result)
        {
            DialogResult = result;
        }

        public ModalDialogResult<JsonCodeResult> DataResult => viewModel.Result;
    }
}
