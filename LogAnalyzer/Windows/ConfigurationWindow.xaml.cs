using Autofac;
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

namespace LogAnalyzer.Windows
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window, IConfigurationWindowAccess
    {
        private ConfigurationWindowViewModel viewModel;

        public ConfigurationWindow()
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<ConfigurationWindowViewModel>(new NamedParameter("access", this));

            DataContext = viewModel;
        }

        public void Close(bool result)
        {
            DialogResult = result;
        }
    }
}
