using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.API.LogParser;
using System.ComponentModel;

namespace LogAnalyzer.BusinessLogic.Models.Views.OpenWindow
{
    public class LogParserProfileInfo : INotifyPropertyChanged
    {
        private bool compatible;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public LogParserProfileInfo(string name, Guid guid, ILogParser parser)
        {
            Name = name;
            Guid = guid;
            Parser = parser;
            compatible = false;
        }

        public string Name { get; }
        public Guid Guid { get; }
        public ILogParser Parser { get; }
        public bool Compatible
        {
            get => compatible; set
            {
                compatible = value;
                OnPropertyChanged(nameof(Compatible));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
