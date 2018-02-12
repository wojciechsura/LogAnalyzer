using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.API.Models
{
    public class LogRecord : INotifyPropertyChanged
    {
        // Private methods ----------------------------------------------------

        private HighlightInfo highlight;

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Public methods -----------------------------------------------------

        public LogRecord(LogEntry logEntry)            
        {
            LogEntry = logEntry;
        }

        // Public properties --------------------------------------------------

        public HighlightInfo Highlight
        {
            get
            {
                return highlight;
            }
            set
            {
                highlight = value;
                OnPropertyChanged(nameof(Highlight));
            }
        }

        public LogEntry LogEntry { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
