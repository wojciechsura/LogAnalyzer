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

namespace LogAnalyzer.Windows
{
    /// <summary>
    /// Interaction logic for LicenseWindow.xaml
    /// </summary>
    public partial class LicenseWindow : Window, ILicenseWindowAccess
    {
        private LicenseWindowViewModel viewModel;

        public LicenseWindow()
        {
            InitializeComponent();

            viewModel = LogAnalyzer.Dependencies.Container.Instance.Resolve<LicenseWindowViewModel>(new ParameterOverride("access", this));
            DataContext = viewModel;
        }
    }
}
