using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.Models.DialogResults;
using LogAnalyzer.Services.Common;
using LogAnalyzer.Models.Views.JumpToTime;
using LogAnalyzer.BusinessLogic.ViewModels.Interfaces;
using System.Windows.Input;
using LogAnalyzer.Wpf.Input;
using LogAnalyzer.Services.Interfaces;
using LogAnalyzer.Common.Tools;

namespace LogAnalyzer.BusinessLogic.ViewModels
{
    public class JumpToTimeWindowViewModel : INotifyPropertyChanged
    {
        // Private fields -----------------------------------------------------

        private string date;

        private ModalDialogResult<JumpToTimeResult> result;
        private readonly IJumpToTimeWindowAccess access;
        private readonly IMessagingService messagingService;

        // Private methods ----------------------------------------------------

        private void DoOk()
        {
            // TODO validate
            if (!DateConverter.Validate(date))
            {
                messagingService.Warn("Invalid date!");
                return;
            }

            result.DialogResult = true;
            result.Result = new JumpToTimeResult { ResultDate = DateConverter.ConvertToDate(date) };
            access.Close(true);
        }

        private void DoCancel()
        {
            access.Close(false);
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Public methods -----------------------------------------------------

        public JumpToTimeWindowViewModel(IJumpToTimeWindowAccess access, JumpToTimeModel model, IMessagingService messagingService)
        {
            this.access = access;
            this.messagingService = messagingService;            

            result = new ModalDialogResult<JumpToTimeResult>();
            date = model.DefaultDate.ToString("yyyy-MM-dd HH:mm:ss.fff");
            OnPropertyChanged(nameof(Date));

            OkCommand = new SimpleCommand((obj) => DoOk());
            CancelCommand = new SimpleCommand((obj) => DoCancel());
        }

        // Public properties --------------------------------------------------

        public event PropertyChangedEventHandler PropertyChanged;

        public string Date
        {
            get => date;
            set => date = value;
        }
        

        public ICommand OkCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public ModalDialogResult<JumpToTimeResult> Result => result;
    }
}
