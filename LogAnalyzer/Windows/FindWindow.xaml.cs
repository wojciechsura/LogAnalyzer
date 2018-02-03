using LogAnalyzer.BusinessLogic.ViewModels;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using LogAnalyzer.Models.DialogResults;
using LogAnalyzer.Models.Engine;
using LogAnalyzer.Models.Views.FindWindow;
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
    /// Interaction logic for FindWindow.xaml
    /// </summary>
    public partial class FindWindow : Window, IFindWindowAccess
    {
        private FindWindowViewModel viewModel;

        public FindWindow(FindModel model)
        {
            InitializeComponent();

            this.viewModel = Dependencies.Container.Instance.Resolve<FindWindowViewModel>(new ParameterOverride("access", this), new ParameterOverride("model", model));
            DataContext = viewModel;
        }

        public void Close(bool dialogResult)
        {
            DialogResult = dialogResult;
        }

        public ModalDialogResult<SearchConfig> DataResult => viewModel.Result;
    }
}
