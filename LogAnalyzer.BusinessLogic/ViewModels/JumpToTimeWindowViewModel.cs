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

namespace LogAnalyzer.BusinessLogic.ViewModels
{
    public class JumpToTimeWindowViewModel : INotifyPropertyChanged
    {
        // Private fields -----------------------------------------------------

        private string year;
        private string month;
        private string day;
        private string hour;
        private string minute;
        private string second;
        private string fraction;

        private ModalDialogResult<JumpToTimeResult> result;
        private readonly IJumpToTimeWindowAccess access;
        private readonly IMessagingService messagingService;

        // Private methods ----------------------------------------------------

        private void DoOk()
        {
            try
            {
                DateTime dateresult = new DateTime(int.Parse(year), 
                    int.Parse(month), 
                    int.Parse(day), 
                    int.Parse(hour), 
                    int.Parse(minute), 
                    int.Parse(second), 
                    int.Parse(fraction));

                result.DialogResult = true;
                result.Result = new JumpToTimeResult { ResultDate = dateresult};
                access.Close(true);
            }
            catch
            {
                messagingService.Warn("Invalid date!");
            }
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
            fraction = model.DefaultDate.Millisecond.ToString("000");
            second = model.DefaultDate.Second.ToString("00");
            minute = model.DefaultDate.Minute.ToString("00");
            hour = model.DefaultDate.Hour.ToString("00");
            day = model.DefaultDate.Day.ToString("00");
            month = model.DefaultDate.Month.ToString("00");
            year = model.DefaultDate.Year.ToString("0000");

            result = new ModalDialogResult<JumpToTimeResult>();

            OkCommand = new SimpleCommand((obj) => DoOk());
            CancelCommand = new SimpleCommand((obj) => DoCancel());
        }

        // Public properties --------------------------------------------------

        public event PropertyChangedEventHandler PropertyChanged;

        public string Hour
        {
            get { return hour; }
            set { hour = value; OnPropertyChanged(nameof(Hour)); }
        }

        public string Minute
        {
            get { return minute; }
            set { minute = value; OnPropertyChanged(nameof(Minute)); }
        }

        public string Second
        {
            get { return second; }
            set { second = value; OnPropertyChanged(nameof(Second)); }
        }

        public string Year
        {
            get { return year; }
            set { year = value; OnPropertyChanged(nameof(Year)); }
        }

        public string Month
        {
            get { return month; }
            set { month = value; OnPropertyChanged(nameof(Month)); }
        }

        public string Day
        {
            get { return day; }
            set { day = value; OnPropertyChanged(nameof(Day)); }
        }

        public string Fraction
        {
            get { return fraction; }
            set { fraction = value; OnPropertyChanged(nameof(Fraction)); }
        }

        public ICommand OkCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public ModalDialogResult<JumpToTimeResult> Result => result;
    }
}
