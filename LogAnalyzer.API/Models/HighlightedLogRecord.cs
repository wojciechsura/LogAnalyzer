using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.API.Models
{
    public class HighlightedLogRecord : LogRecord, INotifyPropertyChanged
    {
        private HighlightInfo highlight;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public HighlightedLogRecord(LogEntry entry, LogMetadata meta)
            : base(entry, meta)
        {

        }

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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
