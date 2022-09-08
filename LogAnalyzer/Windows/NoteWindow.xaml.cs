using Autofac;
using LogAnalyzer.BusinessLogic.ViewModels;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using LogAnalyzer.Models.DialogResults;
using LogAnalyzer.Models.Views.NoteWindow;
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
    /// Interaction logic for NoteWindow.xaml
    /// </summary>
    public partial class NoteWindow : Window, INoteWindowAccess
    {
        private NoteWindowViewModel viewModel;

        public NoteWindow(NoteModel model)
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<NoteWindowViewModel>(new NamedParameter("access", this), new NamedParameter("model", model));
            DataContext = viewModel;
        }

        public ModalDialogResult<NoteResult> DataResult => viewModel.Result;

        public void Close(bool result)
        {
            DialogResult = result;
        }
    }
}
