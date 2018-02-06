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

        private int year;
        private int month;
        private int day;
        private int hour;
        private int minute;
        private int second;
        private int fraction;

        private ModalDialogResult<JumpToTimeResult> result;
        private readonly IJumpToTimeWindowAccess access;
        private readonly IMessagingService messagingService;

        // Private methods ----------------------------------------------------

        private void DoOk()
        {
            try
            {
                DateTime dateresult = new DateTime(year, month, day, hour, minute, second, fraction);

                result.DialogResult = true;
                result.Result = new JumpToTimeResult { ResultDate = dateresult};
                access.Close(true);
            }
            catch
            {
                messagingService.Inform("Invalid date!");
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
            year = model.DefaultDate.Year;
            month = model.DefaultDate.Month;
            day = model.DefaultDate.Day;
            hour = model.DefaultDate.Hour;
            minute = model.DefaultDate.Minute;
            second = model.DefaultDate.Second;
            fraction = model.DefaultDate.Millisecond;

            result = new ModalDialogResult<JumpToTimeResult>();

            OkCommand = new SimpleCommand((obj) => DoOk());
            CancelCommand = new SimpleCommand((obj) => DoCancel());
        }

        // Public properties --------------------------------------------------

        public event PropertyChangedEventHandler PropertyChanged;

        public int Hour
        {
            get { return hour; }
            set { hour = value; OnPropertyChanged(nameof(Hour)); }
        }

        public int Minute
        {
            get { return minute; }
            set { minute = value; OnPropertyChanged(nameof(Minute)); }
        }

        public int Second
        {
            get { return second; }
            set { second = value; OnPropertyChanged(nameof(Second)); }
        }

        public int Year
        {
            get { return year; }
            set { year = value; OnPropertyChanged(nameof(Year)); }
        }

        public int Month
        {
            get { return month; }
            set { month = value; OnPropertyChanged(nameof(Month)); }
        }

        public int Day
        {
            get { return day; }
            set { day = value; OnPropertyChanged(nameof(Day)); }
        }

        public int Fraction
        {
            get { return fraction; }
            set { fraction = value; OnPropertyChanged(nameof(Fraction)); }
        }

        public ICommand OkCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public ModalDialogResult<JumpToTimeResult> Result => result;
    }
}
