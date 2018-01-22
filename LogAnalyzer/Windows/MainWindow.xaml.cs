using Fluent;
using LogAnalyzer.BusinessLogic.ViewModels;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using Unity;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Unity.Resolution;

namespace LogAnalyzer.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow, IMainWindowAccess
    {
        private MainWindowViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<MainWindowViewModel>(new ParameterOverride("access", this));
            DataContext = viewModel;
        }
    }
}
