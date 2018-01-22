using LogAnalyzer.BusinessLogic.ViewModels;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
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
using LogAnalyzer.Services.Common;
using LogAnalyzer.Services.Models.DialolgResults;

namespace LogAnalyzer.Windows
{
    /// <summary>
    /// Interaction logic for ParserProfileEditorWindow.xaml
    /// </summary>
    public partial class ParserProfileEditorWindow : Window, IParserProfileEditorWindowAccess
    {
        private ParserProfileEditorWindowViewModel viewModel;

        public ParserProfileEditorWindow(Guid? editedProfileGuid)
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<ParserProfileEditorWindowViewModel>(new ParameterOverride("access", this), 
                new ParameterOverride("editedProfileGuid", editedProfileGuid));
            DataContext = viewModel;
        }

        public ModalDialogResult<LogParserProfileEditorResult> DataResult => viewModel.Result;
    }
}
