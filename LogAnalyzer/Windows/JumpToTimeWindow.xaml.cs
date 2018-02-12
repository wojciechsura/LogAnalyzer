using LogAnalyzer.BusinessLogic.ViewModels;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using LogAnalyzer.Models.DialogResults;
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
    /// Interaction logic for JumpToTimeWindow.xaml
    /// </summary>
    public partial class JumpToTimeWindow : Window, IJumpToTimeWindowAccess
    {
        private JumpToTimeWindowViewModel viewModel;

        private void JumpToNextTextbox(TextBox current, int desiredLength, TextBox next)
        {
            if (current.Text.Length == desiredLength)
                next.Focus();
        }

        private void tbYear_TextChanged(object sender, TextChangedEventArgs e)
        {
            JumpToNextTextbox(tbYear, 4, tbMonth);
        }

        private void tbMonth_TextChanged(object sender, TextChangedEventArgs e)
        {
            JumpToNextTextbox(tbMonth, 2, tbDay);
        }

        private void tbDay_TextChanged(object sender, TextChangedEventArgs e)
        {
            JumpToNextTextbox(tbDay, 2, tbHour);
        }

        private void tbHour_TextChanged(object sender, TextChangedEventArgs e)
        {
            JumpToNextTextbox(tbHour, 2, tbMinute);
        }

        private void tbMinute_TextChanged(object sender, TextChangedEventArgs e)
        {
            JumpToNextTextbox(tbMinute, 2, tbSecond);
        }

        private void tbSecond_TextChanged(object sender, TextChangedEventArgs e)
        {
            JumpToNextTextbox(tbSecond, 2, tbFraction);
        }

        public JumpToTimeWindow(Models.Views.JumpToTime.JumpToTimeModel model)
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<JumpToTimeWindowViewModel>(new ParameterOverride("access", this), new ParameterOverride("model", model));
            DataContext = viewModel;
        }

        public ModalDialogResult<JumpToTimeResult> DataResult => viewModel.Result;

        public void Close(bool result)
        {
            DialogResult = result;
        }

        private void TextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox)?.SelectAll();
        }
    }
}
