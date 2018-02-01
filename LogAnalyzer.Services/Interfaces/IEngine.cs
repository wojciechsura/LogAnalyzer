using LogAnalyzer.API.Models;
using LogAnalyzer.Models.Engine;
using LogAnalyzer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Services.Interfaces
{
    public class EngineStoppedEventArgs : EventArgs
    {
        public EngineStoppedEventArgs(object stopObject)
        {
            StopObject = stopObject;
        }

        public object StopObject { get; set; }
    }

    public delegate void EngineStoppedDelegate(object sender, EngineStoppedEventArgs args);

    public interface IEngine
    {
        void NotifySourceReady();
        ObservableRangeCollection<HighlightedLogEntry> LogEntries { get; }
        void Stop(Action stopAction);
        List<BaseColumnInfo> GetColumnInfos();

        HighlightConfig HighlightConfig { get; set; }
        FilterConfig FilterConfig { get; set; }
    }
}
