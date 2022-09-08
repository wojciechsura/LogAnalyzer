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
using LogAnalyzer.Services.Common;
using LogAnalyzer.Models.DialogResults;
using Autofac;

namespace LogAnalyzer.Windows
{
    /// <summary>
    /// Interaction logic for ParserProfileEditorWindow.xaml
    /// </summary>
    public partial class ParserProfileEditorWindow : Window, IParserProfileEditorWindowAccess
    {
        private ParserProfileEditorWindowViewModel viewModel;

        public ParserProfileEditorWindow(Guid editedProfileGuid, List<string> sampleLines)
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<ParserProfileEditorWindowViewModel>(
                new NamedParameter("access", this),
                new NamedParameter("editedProfileGuid", editedProfileGuid),
                new NamedParameter("sampleLines", sampleLines));

            DataContext = viewModel;
        }

        public ModalDialogResult<LogParserProfileEditorResult> DataResult => viewModel.Result;

        public void Close(bool dialogResult)
        {
            DialogResult = dialogResult;
        }
    }
}
