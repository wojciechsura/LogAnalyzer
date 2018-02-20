using LogAnalyzer.BusinessLogic.ViewModels;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using LogAnalyzer.Models.Views.LogMessageVisualizerWindow;
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
    /// Interaction logic for LogMessageVisualizerWindow.xaml
    /// </summary>
    public partial class LogMessageVisualizerWindow : Window, ILogMessageVisualizerWindowAccess
    {
        private LogMessageVisualizerWindowViewModel viewModel;

        public LogMessageVisualizerWindow(LogMessageVisualizerModel model)
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<LogMessageVisualizerWindowViewModel>(new ParameterOverride("access", this), new ParameterOverride("model", model));
            DataContext = viewModel;
        }

        public void Display(string html)
        {
            wbBrowser.Navigate(new Uri("http://whatsmybrowser.org"));
            // wbBrowser.NavigateToString(html);
        }
    }
}
