using LogAnalyzer.API.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.API.Models
{
    // For thread-safety this object must stay read-only
    public class LogEntry : BaseLogEntry, INotifyPropertyChanged
    {
        private readonly ILogEntryMetaHandler handler;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
             
        public LogEntry(BaseLogEntry source, int index, ILogEntryMetaHandler handler)
            : base(source.Date, source.Severity, source.Message, source.CustomFields)
        {
            Index = index;
            this.handler = handler;
        }

        public void NotifyBookmarksChanged()
        {
            OnPropertyChanged(nameof(Bookmarks));
        }

        public int Index { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<string> Bookmarks
        {
            get
            {
                return handler.GetBookmarks(this);
            }
        }
    }
}
