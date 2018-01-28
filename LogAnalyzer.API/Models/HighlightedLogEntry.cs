using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.API.Models
{
    public class HighlightedLogEntry : INotifyPropertyChanged
    {
        private HighlightInfo highlight;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public HighlightedLogEntry(FilteredLogEntry entry)
        {
            LogEntry = entry;
        }

        public FilteredLogEntry LogEntry { get; }
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
