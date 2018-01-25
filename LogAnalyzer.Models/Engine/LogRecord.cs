using LogAnalyzer.API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.Engine
{
    public class LogRecord : INotifyPropertyChanged
    {
        private readonly LogEntry logEntry;
        private LogHighlighting logHighlighting;

        private void OnLogHighlightingChanged()
        {
            OnPropertyChanged(nameof(LogHighlighting));
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public LogRecord(LogEntry logEntry)
        {
            this.logEntry = logEntry;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public LogHighlighting LogHighlighting
        {
            get
            {
                return logHighlighting;
            }
            set
            {
                logHighlighting = value;
                OnLogHighlightingChanged();
            }
        }
    }
}
