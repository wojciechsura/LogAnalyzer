using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.API.Models
{
    public class LogMetadata : INotifyPropertyChanged
    {
        private Bookmark bookmark;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public LogMetadata(int index)
        {
            Index = index;
        }

        public int Index { get; }
        public Bookmark Bookmark
        {
            get => bookmark;
            set
            {
                bookmark = value;
                OnPropertyChanged(nameof(Bookmark));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
